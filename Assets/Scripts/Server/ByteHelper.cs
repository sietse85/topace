using System;
using UnityEngine;

namespace Server
{
    public class ByteHelper : MonoBehaviour
    {
        private byte[] float3Buf;
        private byte[] float4Buf;
        private byte[] intBuf;
        private Vector3 tmpVector3;
        private Quaternion tmpQuaternion;
        private int tmpInt;

        private void Start()
        {
            float3Buf = new byte[12];
            float4Buf = new byte[16];
            intBuf = new byte[4];
            tmpVector3 = new Vector3();
            tmpQuaternion = new Quaternion();
        }

        public byte[] Vector3ToByte(Vector3 v)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(v.x), 0, float3Buf, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(v.y), 0, float3Buf, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(v.z), 0, float3Buf, 8, 4);
            return float3Buf;
        }
        
        public byte[] QuaternionToByte(Quaternion q)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(q.x), 0, float4Buf, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(q.y), 0, float4Buf, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(q.z), 0, float4Buf, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(q.w), 0, float4Buf, 12, 4);
            return float4Buf;
        }

        public byte[] IntToByte(int i)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(i), 0, intBuf, 0, 4);
            return intBuf;
        }

        public Vector3 ByteToVector3(byte[] bytes)
        {
            tmpVector3.x = BitConverter.ToSingle(bytes, 0);
            tmpVector3.y = BitConverter.ToSingle(bytes, 4);
            tmpVector3.z = BitConverter.ToSingle(bytes, 8);
            return tmpVector3;
        }

        public Quaternion ByteToQuaternion(byte[] bytes)
        {
            tmpQuaternion.x = BitConverter.ToSingle(bytes, 0);
            tmpQuaternion.y = BitConverter.ToSingle(bytes, 4);
            tmpQuaternion.z = BitConverter.ToSingle(bytes, 8);
            tmpQuaternion.w = BitConverter.ToSingle(bytes, 12);
            return tmpQuaternion;
        }

        public int ByteToInt(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);

        }
    }
}