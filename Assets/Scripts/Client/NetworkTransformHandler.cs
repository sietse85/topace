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
        private Vector3 loc;
        private Quaternion rot;
        private NetworkTransformsForVehicle vnt;
        private byte[] float3Buf;
        private byte[] float4Buf;
        private byte[] transformIdBuf;
        private byte playerIdBuf;
        private int tmpNetworkTransformId;
        private int tmpNetworkTransformPlayerId;

        private void Start()
        {
            float3Buf = new byte[12];
            float4Buf = new byte[16];
            transformIdBuf = new byte[4];
            loc = new Vector3();
            rot = new Quaternion();
            vnt = new NetworkTransformsForVehicle();
            vnt.HeaderByte = HeaderBytes.NetworkTransFormsForVehicle;
        }

        public void UpdateNetworkTransform(NetPacketReader r)
        {
            byte[] bytes = r.GetRemainingBytes();
            int index = 0;
            while (index < bytes.Length)
            {
                Buffer.BlockCopy(bytes, index, float3Buf, 0, sizeof(float) * 3);
                index += sizeof(float) * 3;
                Buffer.BlockCopy(bytes, index, float4Buf, 0, sizeof(float) * 4);
                index += sizeof(float) * 4;
                Buffer.BlockCopy(bytes, index, transformIdBuf, 0, sizeof(int));
                index += sizeof(int);
                playerIdBuf = bytes[index];
                index += sizeof(byte);

                loc = ByteHelper.instance.ByteToVector3(float3Buf);
                rot = ByteHelper.instance.ByteToQuaternion(float4Buf);
                
                tmpNetworkTransformId = ByteHelper.instance.ByteToInt(transformIdBuf);
                tmpNetworkTransformPlayerId = playerIdBuf;
                if (tmpNetworkTransformPlayerId != ClientGameManager.instance.playerId)
                {
                    ClientGameManager.instance.networkTransforms[tmpNetworkTransformId].transform.SetPositionAndRotation(loc, rot);
                }
            }
        }

        public void SetTransformIds(NetPacketReader r)
        {
            vnt.Deserialize(r);
            NetworkTransform[] transforms = ClientGameManager.instance.vehicleEntities[vnt.PlayerId].obj.GetComponentsInChildren<NetworkTransform>();
            int i = 0;
            foreach (NetworkTransform t in transforms)
            {
                t.SetPlayerId(vnt.PlayerId);
                t.SetTransformId(vnt.NetworkTransformIds[i]);
                Debug.Log(vnt.NetworkTransformIds[i]);
                ClientGameManager.instance.networkTransforms[vnt.NetworkTransformIds[i]].transform = t.networkTransform;
                ClientGameManager.instance.networkTransforms[vnt.NetworkTransformIds[i]].slotOccupied = true;
                ClientGameManager.instance.networkTransforms[vnt.NetworkTransformIds[i]].processInTick = true;
                i++;
                if (ClientGameManager.instance.playerId == vnt.PlayerId)
                {
                    t.InitUpdates();
                }
            }
        }
    }
}
