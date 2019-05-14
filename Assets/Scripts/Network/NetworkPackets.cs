using LiteNetLib.Utils;

namespace Network
{
    
    public struct RequestSpawn : INetSerializable
    {
        private byte headerByte;
        public int playerId;

        public RequestSpawn(int playerId, int vehicleId)
        {
            headerByte = HeaderBytes.RequestSpawn;
            this.playerId = playerId;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(headerByte);
            writer.Put(playerId);
        }

        public void Deserialize(NetDataReader reader)
        {
            playerId = reader.GetInt();
        }
    }

    public struct SpawnShip : INetSerializable
    {
        private byte headerByte;
        private int playerId;
        private int prefabNr;
        private int shipId;
        private float posx;
        private float posy;
        private float posz;
        private float rotx;
        private float roty;
        private float rotz;

        public SpawnShip(int playerId, int prefabNr, int shipId, float posx, float posy, float posz, float rotx,
            float roty, float rotz)
        {
            headerByte = 0x05;
            this.playerId = playerId;
            this.prefabNr = prefabNr;
            this.shipId = shipId;
            this.posx = posx;
            this.posy = posy;
            this.posz = posz;
            this.rotx = rotx;
            this.roty = roty;
            this.rotz = rotz;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(headerByte);
            writer.Put(playerId);
            writer.Put(prefabNr);
            writer.Put(shipId);
            writer.Put(posx);
            writer.Put(posy);
            writer.Put(posz);
            writer.Put(rotx);
            writer.Put(roty);
            writer.Put(rotz);
        }

        public void Deserialize(NetDataReader reader)
        {
            headerByte = reader.GetByte();
            prefabNr = reader.GetInt();
            shipId = reader.GetInt();
            posx = reader.GetFloat();
            posy = reader.GetFloat();
            posz = reader.GetFloat();
            rotx = reader.GetFloat();
            roty = reader.GetFloat();
            rotz = reader.GetFloat();
        }
    }
    
    public struct AskForUserName : INetSerializable
    {
        public byte headerByte;

        public AskForUserName(byte header)
        {
            headerByte = header;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(headerByte);
        }

        public void Deserialize(NetDataReader reader)
        {
            reader.GetByte();
        }
    }
    
    public struct SendUserName : INetSerializable
    {
        private byte headerByte;
        public string playerName;

        public SendUserName(string name)
        {
            headerByte = 0x02;
            playerName = name;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(headerByte);
            writer.Put(playerName);
        }

        public void Deserialize(NetDataReader reader)
        {
            playerName = reader.GetString();
        }
    }
    
    public struct SendPlayerId : INetSerializable
    {
        private byte headerByte;
        private int playerId;

        public SendPlayerId(int playerId)
        {
            headerByte = 0x04;
            this.playerId = playerId;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(headerByte);
            writer.Put(playerId);
        }

        public void Deserialize(NetDataReader reader)
        {
            headerByte = reader.GetByte();
            playerId = reader.GetInt();
        }
    }
}