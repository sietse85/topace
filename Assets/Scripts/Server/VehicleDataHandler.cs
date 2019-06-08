using System;
using LiteNetLib;
using UnityEngine;
using Network;

namespace Server
{
    public class VehicleDataHandler : MonoBehaviour
    {
        private Vector3 _tmpPos;
        private Quaternion _tmpRot;
        private NetworkTransformUpdate u;
        private Ticker _ticker;

        private void Start()
        {
            GameManager.instance = GetComponent<GameManager>();
            _ticker = GetComponent<Ticker>();
            _tmpPos = new Vector3();
            _tmpRot = new Quaternion();
        }

        public void ClientRequestedVehicleSpawn(NetPeer peer, NetPacketReader r)
        {
            u = new NetworkTransformUpdate();
            RequestSpawn packet = new RequestSpawn();
            packet.Deserialize(r);
            if (GameManager.instance.players[packet.PlayerId].securityPin != packet.PlayerPin)
            {
                return;
            }
            Byte[] b = new byte[1];
            b[0] = HeaderBytes.OpenSpawnMenuOnClient;
            peer.Send(b, DeliveryMethod.ReliableUnordered);
            u.HeaderByte = HeaderBytes.NetworkTransFormId;
            GameManager.instance.vc.ConstructVehicle(packet.PlayerId, packet.VehicleDatabaseId, Vector3.zero, Quaternion.identity, packet.Config);
        }

        public void UpdateVehicleTransform(NetPacketReader r)
        {
            u.Deserialize(r);
            //stops players from sending networktransforms of other players
            if (GameManager.instance.players[u.PlayerId].securityPin != u.PlayerPin)
            {
                return;
            } 
            _tmpPos.x = u.LocX;
            _tmpPos.y = u.LocY;
            _tmpPos.z = u.LocZ;
            _tmpRot.x = u.RotX;
            _tmpRot.y = u.RotY;
            _tmpRot.z = u.RotZ;
            _tmpRot.w = u.RotW;
            _ticker.networkTransforms[u.NetworkTransformId].transform.SetPositionAndRotation(_tmpPos, _tmpRot);
            
        }
        
        public void SendRemoveVehicleByPlayerId(byte playerId)
        {
            RemoveVehicle rv = new RemoveVehicle(playerId);
            GameServer.instance.SendToAll(rv); 
        }
    }
}