using System;
using LiteNetLib;
using Network;
using Server;
using UnityEngine;

namespace Client
{
    public class NetworkTransformHandler : MonoBehaviour
    {
        // Start is called before the first frame update
        private GameClient _gameClient;
        private ClientGameManager _game;
        private Vector3 loc;
        private Quaternion rot;
        private NetworkTransformsForVehicle vnt;
        private byte[] float3Buf;
        private byte[] float4Buf;
        private byte[] transformIdBuf;
        private byte[] playerIdBuf;
        private int tmpNetworkTransformId;
        private int tmpNetworkTransformPlayerId;
        private ByteHelper b;

        private void Start()
        {
            float3Buf = new byte[12];
            float4Buf = new byte[16];
            transformIdBuf = new byte[4];
            playerIdBuf = new byte[4];
            _gameClient = GetComponent<GameClient>();
            _game = GetComponent<ClientGameManager>();
            loc = new Vector3();
            rot = new Quaternion();
            vnt = new NetworkTransformsForVehicle();
            vnt.HeaderByte = HeaderBytes.NetworkTransFormsForVehicle;
            b = gameObject.AddComponent<ByteHelper>();
        }

        public void UpdateNetworkTransform(NetPacketReader r)
        {
            byte[] bytes = r.GetRemainingBytes();
            int index = 0;
            while (bytes[index] != 0x00)
            {
                Buffer.BlockCopy(bytes, index, float3Buf, 0, sizeof(float) * 3);
                index += sizeof(float) * 3;
                Buffer.BlockCopy(bytes, index, float4Buf, 0, sizeof(float) * 4);
                index += sizeof(float) * 4;
                Buffer.BlockCopy(bytes, index, transformIdBuf, 0, sizeof(int));
                index += sizeof(int);
                Buffer.BlockCopy(bytes, index, playerIdBuf, 0, sizeof(int));

                loc = b.ByteToVector3(float3Buf);
                rot = b.ByteToQuaternion(float4Buf);
                tmpNetworkTransformId = b.ByteToInt(transformIdBuf);
                tmpNetworkTransformPlayerId = b.ByteToInt(playerIdBuf);
                if (tmpNetworkTransformPlayerId != _game.playerId)
                {
                    _game.networkTransforms[tmpNetworkTransformId].transform.SetPositionAndRotation(loc, rot);
                }
            }
        }

        public void SetTransformIds(NetPacketReader r)
        {
            Debug.Log("set transform ids");
            vnt.Deserialize(r);
            

            NetworkTransform[] transforms = _game.VehicleEntities[vnt.PlayerId].obj.GetComponentsInChildren<NetworkTransform>();

            int i = 0;

            foreach (NetworkTransform t in transforms)
            {
                t.SetPlayerId(vnt.PlayerId);
                t.SetTransformId(vnt.NetworkTransformIds[i]);
                Debug.Log(vnt.NetworkTransformIds[i]);
                _game.networkTransforms[vnt.NetworkTransformIds[i]].transform = t.networkTransform;
                _game.networkTransforms[vnt.NetworkTransformIds[i]].slotOccupied = true;
                _game.networkTransforms[vnt.NetworkTransformIds[i]].processInTick = true;
                i++;
                if (_game.playerId == vnt.PlayerId)
                {
                    t.InitUpdates();
                }
            }
        }
    }
}
