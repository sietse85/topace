using LiteNetLib;

namespace Server
{
    public struct Player
    {
        public bool slotOccupied;
        public int latency;
        public int score;
        public int kills;
        public int deaths;
        public int shotsFired;
        public int shotsHit;
        public bool processInTick;
        public NetPeer peer;
        public string playerName;
    }
}