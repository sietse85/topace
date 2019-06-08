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
        private Vector3 _loc;
        private Quaternion _rot;
        private NetworkTransformsForVehicle _vnt;
        private byte[] _float3Buf;
        private byte[] _float4Buf;
        private byte[] _transformIdBuf;
        private byte _playerIdBuf;
        private int _tmpNetworkTransformId;
        private int _tmpNetworkTransformPlayerId;

        private void Start()
        {
            _float3Buf = new byte[12];
            _float4Buf = new byte[16];
            _transformIdBuf = new byte[4];
            _loc = new Vector3();
            _rot = new Quaternion();
            _vnt = new NetworkTransformsForVehicle();
            _vnt.HeaderByte = HeaderBytes.NetworkTransFormsForVehicle;
        }

        public void UpdateNetworkTransform(ref byte[] snapshot)
        {
            int index = ClientGameManager.instance.index;
            Buffer.BlockCopy(snapshot, index, _float3Buf, 0, sizeof(float) * 3);
            index += sizeof(float) * 3;
            Buffer.BlockCopy(snapshot, index, _float4Buf, 0, sizeof(float) * 4);
            index += sizeof(float) * 4;
            Buffer.BlockCopy(snapshot, index, _transformIdBuf, 0, sizeof(int));
            index += sizeof(int);
            _playerIdBuf = snapshot[index];
            index += sizeof(byte);

            _loc = ByteHelper.instance.ByteToVector3(_float3Buf);
            _rot = ByteHelper.instance.ByteToQuaternion(_float4Buf);
            
            _tmpNetworkTransformId = ByteHelper.instance.ByteToInt(_transformIdBuf);
            _tmpNetworkTransformPlayerId = _playerIdBuf;
            if (_tmpNetworkTransformPlayerId != ClientGameManager.instance.playerId)
            {
                ClientGameManager.instance.networkTransforms[_tmpNetworkTransformId].transform.SetPositionAndRotation(_loc, _rot);
            }

            ClientGameManager.instance.index = index;
        }

        public void SetTransformIds(NetPacketReader r)
        {
            _vnt.Deserialize(r);
            NetworkTransform[] transforms = ClientGameManager.instance.vehicleEntities[_vnt.PlayerId].obj.GetComponentsInChildren<NetworkTransform>();
            int i = 0;
            foreach (NetworkTransform t in transforms)
            {
                t.SetPlayerId(_vnt.PlayerId);
                t.SetTransformId(_vnt.NetworkTransformIds[i]);
                ClientGameManager.instance.networkTransforms[_vnt.NetworkTransformIds[i]].transform = t.networkTransform;
                ClientGameManager.instance.networkTransforms[_vnt.NetworkTransformIds[i]].slotOccupied = true;
                ClientGameManager.instance.networkTransforms[_vnt.NetworkTransformIds[i]].processInTick = true;
                i++;
                if (ClientGameManager.instance.playerId == _vnt.PlayerId)
                {
                    t.InitUpdates();
                }
            }
        }
    }
}
