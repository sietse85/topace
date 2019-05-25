using System;
using LiteNetLib;
using UnityEngine;
using Network;

namespace Server
{
    public class VehicleDataHandler : MonoBehaviour
    {
        private Server _server;
        private GameManager _game;

        private Vector3 tmpPos;
        private Quaternion tmpRot;

        private NetworkTransformUpdate u;

        private void Start()
        {
            _server = GetComponent<Server>();
            _game = GetComponent<GameManager>();
            tmpPos = new Vector3();
            tmpRot = new Quaternion();
        }

        public void PlayerRequestedShipSpawn(NetPeer peer, NetPacketReader r)
        {
            u = new NetworkTransformUpdate();
            RequestSpawn packet = new RequestSpawn();
            packet.Deserialize(r);
            Byte[] b = new byte[1];
            b[0] = HeaderBytes.OpenSpawnMenuOnClient;
            peer.Send(b, DeliveryMethod.ReliableUnordered);
            u.HeaderByte = HeaderBytes.NetworkTransFormId;
            _game.vc.ConstructVehicle(packet.PlayerId, packet.VehicleId, packet.VehicleId, Vector3.zero, Quaternion.identity, packet.Config);
        }

        public void UpdateVehicleTransform(NetPacketReader r)
        {
            u.Deserialize(r);
            tmpPos.x = u.LocX;
            tmpPos.y = u.LocY;
            tmpPos.z = u.LocZ;
            tmpRot.x = u.RotX;
            tmpRot.y = u.RotY;
            tmpRot.z = u.RotZ;
            tmpRot.w = u.RotW;
            _game.networkTransforms[u.NetworkTransformId].networkTransform.SetPositionAndRotation(tmpPos, tmpRot);
        }
    }
}