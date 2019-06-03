using LiteNetLib;
using UnityEngine;
using Network;

namespace Server
{
    public class PlayerDataHandler : MonoBehaviour
    {
       public byte GetNextAvailablePlayerSlot()
        {
            for (byte i = 0; i < GameManager.instance.players.Length; i++)
            {
                if (!GameManager.instance.players[i].slotOccupied)
                {
                    GameManager.instance.players[i].slotOccupied = true;
                    Debug.Log("The next free playerslot = " + i);
                    return i;
                }
            }
            // 0xff 255 all slots are occupied
            return 0xff;
        }

        public void ReceivePlayerName(NetPeer peer, NetPacketReader r)
        {
            SendUserNameToServer n = new SendUserNameToServer();
            n.Deserialize(r);
            byte playerId = GetNextAvailablePlayerSlot();
            System.Random rnd = new System.Random();
            int securityPin = rnd.Next(0, 2147483647);
            GameManager.instance.players[playerId].playerName = n.PlayerName;
            GameManager.instance.players[playerId].peer = peer;
            GameManager.instance.players[playerId].securityPin = securityPin;
            GameManager.instance.players[playerId].processInTick = true;
            SendPlayerId(peer, playerId, securityPin);
            GameManager.instance.vc.SpawnExistingVehiclesOnClient(peer, playerId);
        }

        public void SendPlayerId(NetPeer peer, byte playerId, int securityPin)
        {
            SendPlayerId packet = new SendPlayerId(playerId, securityPin);
            GameServer.instance.Send(packet, peer);
        }

        public void RemovePlayer(NetPeer peer)
        {
            for (byte i = 0; i < GameManager.instance.players.Length; i++)
            {
                if (GameManager.instance.players[i].peer == null)
                    continue;
                
                if (GameManager.instance.players[i].peer.Id == peer.Id)
                {
                    Debug.Log("Player was disconnected, freeing slot and check if vehicle and networktransforms must be removed");
                    // free this player slot
                    GameManager.instance.players[i].processInTick = false;
                    GameManager.instance.players[i].slotOccupied = false;
                    GameManager.instance.players[i].peer = null;
                    // remove the player from the namelist
                    GameManager.instance.playerNames.Remove(i);
                    // remove the vehicle if it not used by anyone anymore
                    if (!GameManager.instance.vehicleEntities[i].isOccupiedByOtherPlayer)
                    {
                        GameManager.instance.vehicleEntities[i].processInTick = false;
                        NetworkTransform[] networkTransforms =
                        GameManager.instance.vehicleEntities[i].obj.GetComponentsInChildren<NetworkTransform>();
                        foreach (NetworkTransform t in networkTransforms)
                        {
                            Debug.Log("Removing networktransformId: " + t.networkTransformId);
                            GameManager.instance.ticker.networkTransforms[t.networkTransformId].slotOccupied = false;
                            GameManager.instance.ticker.networkTransforms[t.networkTransformId].processInTick = false;
                        }
                        Destroy(GameManager.instance.vehicleEntities[i].obj);
                        GameManager.instance.vehicleEntities[i].processInTick = false;
                        Debug.Log("Sending the players to remove the vehicle for " + i);
                        GameManager.instance.vehicleDataHandler.SendRemoveVehicleByPlayerId(i);
                    }
                }
            }
        }

        public void UpdateLatencyOfPlayer(NetPeer peer, int latency)
        {
            for (int i = 0; i < GameManager.instance.players.Length; i++)
            {
                if (GameManager.instance.players[i].slotOccupied)
                {
                    if (GameManager.instance.players[i].peer.Id == peer.Id)
                    {
                        GameManager.instance.players[i].latency = latency;
                    }
                }
            }
        }

        public void HandleNewConnection(NetPeer peer)
        {
            AskClientForUsername askname = new AskClientForUsername(0x01);
            GameServer.instance.Send(askname, peer);
        }
    }
}