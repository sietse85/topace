using UnityEngine;

namespace Network
{
    public struct NetworkTransformStruct
    {
        public byte playerId;
        public bool isMain;
        public bool processInTick;
        public bool slotOccupied;
        public Transform transform;
    }
}