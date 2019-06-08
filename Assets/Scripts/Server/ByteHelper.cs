using System;
using UnityEngine;

namespace Server
{
    public class ByteHelper : MonoBehaviour
    {
        public static ByteHelper instance;
        
        private byte[] _float3Buf;
        private byte[] _float4Buf;
        private byte[] _intBuf;
        private byte[] _floatBuf;
        private Vector3 _tmpVector3;
        private Quaternion _tmpQuaternion;
        private int _tmpInt;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            _float3Buf = new byte[12];
            _float4Buf = new byte[16];
            _intBuf = new byte[4];
            _floatBuf = new byte[4];
            _tmpVector3 = new Vector3();
            _tmpQuaternion = new Quaternion();
        }

        public byte[] Vector3ToByte(Vector3 v)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(v.x), 0, _float3Buf, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(v.y), 0, _float3Buf, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(v.z), 0, _float3Buf, 8, 4);
            return _float3Buf;
        }
        
        public byte[] QuaternionToByte(Quaternion q)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(q.x), 0, _float4Buf, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(q.y), 0, _float4Buf, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(q.z), 0, _float4Buf, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(q.w), 0, _float4Buf, 12, 4);
            return _float4Buf;
        }

        public byte[] FloatToByte(float f)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(f),0, _floatBuf, 0, 4);
            return _floatBuf;
        }

        public byte[] IntToByte(int i)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(i), 0, _intBuf, 0, 4);
            return _intBuf;
        }

        public Vector3 ByteToVector3(byte[] bytes)
        {
            _tmpVector3.x = BitConverter.ToSingle(bytes, 0);
            _tmpVector3.y = BitConverter.ToSingle(bytes, 4);
            _tmpVector3.z = BitConverter.ToSingle(bytes, 8);
            return _tmpVector3;
        }

        public Quaternion ByteToQuaternion(byte[] bytes)
        {
            _tmpQuaternion.x = BitConverter.ToSingle(bytes, 0);
            _tmpQuaternion.y = BitConverter.ToSingle(bytes, 4);
            _tmpQuaternion.z = BitConverter.ToSingle(bytes, 8);
            _tmpQuaternion.w = BitConverter.ToSingle(bytes, 12);
            return _tmpQuaternion;
        }

        public float ByteToFloat(byte[] bytes)
        {
            return BitConverter.ToSingle(bytes,0);
        }

        public int ByteToInt(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}