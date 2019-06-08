using System;
using LiteNetLib;
using Resource;
using Scriptable;
using Server;
using UnityEngine;

namespace Client
{
    public class ProjectileHandler : MonoBehaviour
    {

        private Vector3 _pos;
        private Quaternion _rot;
        private int _projectileId;
        private byte _uniqueProjectileId;
        private byte _vehicleId;

        private byte[] _intBuf;
        private byte[] _float3Buf;
        private byte[] _float4Buf;

        private void Awake()
        {
            _intBuf = new byte[4];
            _float3Buf = new byte[12];
            _float4Buf = new byte[16];
        }

        public void SpawnProjectile(ref byte[] snapshot)
        {

            int index = ClientGameManager.instance.index;
            
            Buffer.BlockCopy(snapshot, index, _intBuf, 0, sizeof(int));
            _projectileId = ByteHelper.instance.ByteToInt(_intBuf);
            index += sizeof(int);
            _vehicleId = snapshot[index];
            index += sizeof(byte);
            _uniqueProjectileId = snapshot[index];
            index += sizeof(byte);
            Buffer.BlockCopy(snapshot, index, _float3Buf, 0, sizeof(float) * 3);
            _pos = ByteHelper.instance.ByteToVector3(_float3Buf);
            index += sizeof(float) * 3;
            Buffer.BlockCopy(snapshot, index, _float4Buf, 0, sizeof(float) * 4);
            _rot = ByteHelper.instance.ByteToQuaternion(_float4Buf);
            index += sizeof(float) * 4;
            
            InstantiateProjectile(_pos, _rot, _projectileId, _uniqueProjectileId, _vehicleId);
            
            ClientGameManager.instance.index = index;
        }

        public void InstantiateProjectile(Vector3 pos, Quaternion rot, int projectileId, byte uniqueProjectileId,
            byte vehicleId)
        {
            if (vehicleId == ClientGameManager.instance.playerId)
            {
                return;
            }
            
            Projectile projectile = Loader.instance.projectiles[projectileId];

            GameObject obj = Instantiate(
                projectile.prefab,
                pos,
                rot
            );
            
            ProjectileEntity p = obj.GetComponent<ProjectileEntity>();
            p.doRayCast = false;
            p.timeToLive = projectile.timeToLive;
            p.velocity = projectile.projectileSpeed;
            p.uniqueProjectileId = uniqueProjectileId;
            p.projectileDataBaseId = projectileId;

            ClientGameManager.instance.projectiles[vehicleId * 100 + uniqueProjectileId].obj = obj;
            ClientGameManager.instance.projectiles[vehicleId * 100 + uniqueProjectileId].active = true;
        }
    }
}
