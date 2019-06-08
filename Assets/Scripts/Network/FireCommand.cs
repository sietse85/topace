using UnityEngine;

namespace Network
{
    public struct FireCommand
    {
        public byte WeaponSlotFired;
        public byte VehicleId;
        public int ProjectileId;
        public byte UniqueProjectileId;
        public bool Process;
        public Vector3 Rotation;
    }
}