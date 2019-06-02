using System;
using LiteNetLib;
using UnityEngine;
using Network;

namespace Server
{
    public class VehicleDataHandler : MonoBehaviour
    {
        private GameManager _game;

        private Vector3 tmpPos;
        private Quaternion tmpRot;

        private NetworkTransformUpdate u;
        private Ticker _ticker;

        private void Start()
        {
            _game = GetComponent<GameManager>();
            _ticker = GetComponent<Ticker>();
            tmpPos = new Vector3();
            tmpRot = new Quaternion();
        }

        public void ClientRequestedVehicleSpawn(NetPeer peer, NetPacketReader r)
        {
            u = new NetworkTransformUpdate();
            RequestSpawn packet = new RequestSpawn();
            packet.Deserialize(r);
            if (_game.players[packet.PlayerId].securityPin != packet.PlayerPin)
            {
                return;
            }
            Byte[] b = new byte[1];
            b[0] = HeaderBytes.OpenSpawnMenuOnClient;
            peer.Send(b, DeliveryMethod.ReliableUnordered);
            u.HeaderByte = HeaderBytes.NetworkTransFormId;
            _game.vc.ConstructVehicle(packet.PlayerId, packet.VehicleDatabaseId, Vector3.zero, Quaternion.identity, packet.Config);
        }

        public void UpdateVehicleTransform(NetPacketReader r)
        {
            u.Deserialize(r);
            //stops players from sending networktransforms of other players
            if (_game.players[u.PlayerId].securityPin != u.PlayerPin)
            {
                Debug.Log(_game.players[u.PlayerId].securityPin + " pin does not match "  + u.PlayerPin);
                return;
            } 
            tmpPos.x = u.LocX;
            tmpPos.y = u.LocY;
            tmpPos.z = u.LocZ;
            tmpRot.x = u.RotX;
            tmpRot.y = u.RotY;
            tmpRot.z = u.RotZ;
            tmpRot.w = u.RotW;
            _ticker.networkTransforms[u.NetworkTransformId].transform.SetPositionAndRotation(tmpPos, tmpRot);
        }
        
        public void SendRemoveVehicleByPlayerId(byte playerId)
        {
            RemoveVehicle rv = new RemoveVehicle(playerId);
            _game.gameServer.SendToAll(rv); 
        }
    }
}