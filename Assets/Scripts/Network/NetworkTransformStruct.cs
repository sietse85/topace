using UnityEngine;

namespace Network
{
    public struct NetworkTransformStruct
    {
        public Vector3 position;
        public Quaternion rotation;
        public int playerId;
        public int networkTransformId;
        public bool processInTick;
        public bool slotOccupied;
        public Transform transform;
    }
}