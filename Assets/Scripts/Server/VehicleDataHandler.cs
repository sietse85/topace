using System;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using UnityEngine;

namespace Network
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
            u = new NetworkTransformUpdate();
        }

        public void PlayerRequestedShipSpawn(NetPeer peer, NetPacketReader r)
        {
            u = new NetworkTransformUpdate();
            RequestSpawn packet = new RequestSpawn();
            packet.Deserialize(r);
            Byte[] b = new byte[1];
            b[0] = HeaderBytes.OpenSpawnMenuOnClient;
            peer.Send(b, DeliveryMethod.ReliableUnordered);
            Debug.Log("Sending open spawn menu");
            u.headerByte = HeaderBytes.NetworkTransFormId;
            
            _game.vc.ConstructVehicle(packet.playerId, packet.vehicleId, Vector3.zero, Quaternion.identity, packet.config);

            StartCoroutine(SendActualNetworkTransformPositionsToClients());

        }

        public void UpdateVehicleTransform(NetPacketReader r)
        {
            u.Deserialize(r);

            if (_game.networkTransforms.ContainsKey(u.networkTransformId))
            {
                    tmpPos.x = u.locX;
                    tmpPos.y = u.locY;
                    tmpPos.z = u.locZ;
                    tmpRot.x = u.rotX;
                    tmpRot.y = u.rotY;
                    tmpRot.z = u.rotZ;
                    tmpRot.w = u.rotW;
                    
                    _game.networkTransforms[u.networkTransformId].t.SetPositionAndRotation(tmpPos, tmpRot);
            }
        }

        public IEnumerator SendActualNetworkTransformPositionsToClients()
        {
            while (true)
            {
                if (_game.networkTransforms.Count > 0)
                {
                    foreach (KeyValuePair<int, NetworkTransform> t in _game.networkTransforms)
                    {
                        if (t.Value.t == null)
                            continue;
                       
                        u.locX = t.Value.t.position.x;
                        u.locY = t.Value.t.position.y;
                        u.locZ = t.Value.t.position.z;
                        u.rotX = t.Value.t.rotation.x;
                        u.rotY = t.Value.t.rotation.y;
                        u.rotZ = t.Value.t.rotation.z;
                        u.rotW = t.Value.t.rotation.w;

                        u.networkTransformId = t.Value.networkTransformId;
                        
                        _server.SendToAll(u);
                    }
                }

                yield return new WaitForSeconds(0.033f);
            }
        }
    }
}