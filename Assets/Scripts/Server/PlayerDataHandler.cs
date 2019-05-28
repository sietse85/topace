using System.Collections.Generic;
using LiteNetLib;
using UnityEngine;
using Network;

namespace Server
{
    public class PlayerDataHandler : MonoBehaviour
    {

        private GameServer _gameServer;
        private GameManager game;

        private void Start()
        {
            _gameServer = GetComponent<GameServer>();
            game = GetComponent<GameManager>();
        }

        public int GetNextAvailablePlayerSlot()
        {
            for (int i = 0; i < game.players.Length; i++)
            {
                if (!game.players[i].slotOccupied)
                {
                    game.players[i].slotOccupied = true;
                    Debug.Log("The next free playerslot = " + i);
                    return i;
                }
            }
            // 999 all slots are occupied
            return 999;
        }

        public void ReceivePlayerName(NetPeer peer, NetPacketReader r)
        {
            SendUserNameToServer n = new SendUserNameToServer();
            n.Deserialize(r);
            int playerId = GetNextAvailablePlayerSlot();
            game.players[playerId].playerName = n.PlayerName;
            game.players[playerId].peer = peer;
            game.players[playerId].processInTick = true;
            SendPlayerId(peer, playerId);
            game.vc.SpawnExistingVehiclesOnClient(peer, playerId);
        }

        public void SendPlayerId(NetPeer peer, int playerId)
        {
            SendPlayerId packet = new SendPlayerId(playerId);
            _gameServer.Send(packet, peer);
        }

        public void RemovePlayer(NetPeer peer)
        {
            for (int i = 0; i < game.players.Length; i++)
            {
                if (game.players[i].peer.Id == peer.Id)
                {
                    Debug.Log("Player was disconnected, freeing slot and check if vehicle and networktransforms must be removed");
                    // free this player slot
                    game.players[i].processInTick = false;
                    game.players[i].slotOccupied = false;
                    // remove the player from the namelist
                    game.playerNames.Remove(i);
                    // remove the vehicle if it not used by anyone anymore
                    if (!game.VehicleEntities[i].isOccupiedByOtherPlayer)
                    {
                        game.VehicleEntities[i].processInTick = false;
                        NetworkTransform[] networkTransforms =
                        game.vehicles[i].GetComponentsInChildren<NetworkTransform>();
                        foreach (NetworkTransform t in networkTransforms)
                        {
                            Debug.Log("Removing networktransformId: " + t.networkTransformId);
                            if (game.ticker.networkTransforms.ContainsKey(t.networkTransformId))
                            {
                                game.ticker.networkTransforms.Remove(t.networkTransformId);
                            }
                        }
                        Destroy(game.vehicles[i]);
                        game.vehicles.Remove(i);
                        Debug.Log("Sending the players to remove the vehicle for " + i);
                        game.vehicleDataHandler.SendRemoveVehicleByPlayerId(i);
                    }
                }
            }
        }

        public void UpdateLatencyOfPlayer(NetPeer peer, int latency)
        {
            for (int i = 0; i < game.players.Length; i++)
            {
                if (game.players[i].slotOccupied)
                {
                    if (game.players[i].peer.Id == peer.Id)
                    {
                        game.players[i].latency = latency;
                    }
                }
            }
        }

        public void HandleNewConnection(NetPeer peer)
        {
            AskClientForUsername askname = new AskClientForUsername(0x01);
            _gameServer.Send(askname, peer);
        }
    }
}