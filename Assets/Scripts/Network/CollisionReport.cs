using UnityEngine;

namespace Network
{
    public struct CollisionReport
    {
        public bool Process;
        public byte PlayerIdThatWasHit;
        public byte PlayerIdThatShotThisProjectile;
        public int ByProjectileDatabaseId;
        public byte UniqueProjectileId;
        public byte TickWhenCollisionOccurred;
        public Vector3 impactPos;
    }
}