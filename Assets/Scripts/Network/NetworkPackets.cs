using LiteNetLib.Utils;

namespace Network
{
    
    public struct RequestSpawn : INetSerializable
    {
        public byte HeaderByte;
        public int PlayerId;
        public int VehicleDatabaseId;
        public byte[] Config;

        public RequestSpawn(int playerId, int vehicleDatabaseId, byte[] config)
        {
            HeaderByte = HeaderBytes.RequestSpawn;
            this.PlayerId = playerId;
            this.VehicleDatabaseId = vehicleDatabaseId;
            this.Config = config;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(HeaderByte);
            writer.Put(PlayerId);
            writer.Put(VehicleDatabaseId);
            writer.Put(Config);
        }

        public void Deserialize(NetDataReader reader)
        {
            PlayerId = reader.GetInt();
            VehicleDatabaseId = reader.GetInt();
            Config = reader.GetRemainingBytes();
        }
    }

    public struct SpawnVehicle : INetSerializable
    {
        public byte HeaderByte;
        public int PlayerId;
        public int VehicleDatabaseId;
        public float PosX;
        public float PosY;
        public float PosZ;
        public float RotX;
        public float RotY;
        public float RotZ;
        public float RotW;
        public byte[] Config;
        

        public SpawnVehicle(int playerId, int vehicleDatabaseId, float posX, float posY, float posZ, float rotX,
            float rotY, float rotZ, float rotW, byte[] config)
        {
            HeaderByte = HeaderBytes.SpawnVehicle;
            this.PlayerId = playerId;
            this.VehicleDatabaseId = vehicleDatabaseId;
            this.PosX = posX;
            this.PosY = posY;
            this.PosZ = posZ;
            this.RotX = rotX;
            this.RotY = rotY;
            this.RotZ = rotZ;
            this.RotW = rotW;
            this.Config = config;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(HeaderByte);
            writer.Put(PlayerId);
            writer.Put(VehicleDatabaseId);
            writer.Put(PosX);
            writer.Put(PosY);
            writer.Put(PosZ);
            writer.Put(RotX);
            writer.Put(RotY);
            writer.Put(RotZ);
            writer.Put(RotW);
            if (Config.Length == 0)
            {
               writer.PutBytesWithLength(new byte[8]); 
            }
            else
            {
                writer.Put(Config);
            }
        }

        public void Deserialize(NetDataReader reader)
        {
            PlayerId = reader.GetInt();
            VehicleDatabaseId = reader.GetInt();
            PosX = reader.GetFloat();
            PosY = reader.GetFloat();
            PosZ = reader.GetFloat();
            RotX = reader.GetFloat();
            RotY = reader.GetFloat();
            RotZ = reader.GetFloat();
            RotW = reader.GetFloat();
            Config = reader.GetRemainingBytes();
        }
    }
    
    public struct AskClientForUsername : INetSerializable
    {
        public byte HeaderByte;

        public AskClientForUsername(byte header)
        {
            HeaderByte = header;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(HeaderByte);
        }

        public void Deserialize(NetDataReader reader)
        {
            reader.GetByte();
        }
    }
    
    public struct SendUserNameToServer : INetSerializable
    {
        public byte HeaderByte;
        public string PlayerName;

        public SendUserNameToServer(string name)
        {
            HeaderByte = 0x02;
            PlayerName = name;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(HeaderByte);
            writer.Put(PlayerName);
        }

        public void Deserialize(NetDataReader reader)
        {
            PlayerName = reader.GetString();
        }
    }
    
    public struct SendPlayerId : INetSerializable
    {
        public byte HeaderByte;
        public int PlayerId;

        public SendPlayerId(int playerId)
        {
            HeaderByte = 0x04;
            this.PlayerId = playerId;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(HeaderByte);
            writer.Put(PlayerId);
        }

        public void Deserialize(NetDataReader reader)
        {
            HeaderByte = reader.GetByte();
            PlayerId = reader.GetInt();
        }
    }
    
    public struct NetworkTransformUpdate : INetSerializable
    {
        public byte HeaderByte;
        public float LocX;
        public float LocY;
        public float LocZ;
        public float RotX;
        public float RotY;
        public float RotZ;
        public float RotW;
        public int NetworkTransformId;
        public int PlayerId;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(HeaderBytes.NetworkTransFormId);
            writer.Put(LocX);
            writer.Put(LocY);
            writer.Put(LocZ);
            writer.Put(RotX);
            writer.Put(RotY);
            writer.Put(RotZ);
            writer.Put(RotW);
            writer.Put(NetworkTransformId);
            writer.Put(PlayerId);
        }

        public void Deserialize(NetDataReader reader)
        {
            LocX = reader.GetFloat();
            LocY = reader.GetFloat();
            LocZ = reader.GetFloat();
            RotX = reader.GetFloat();
            RotY = reader.GetFloat();
            RotZ = reader.GetFloat();
            RotW = reader.GetFloat();
            NetworkTransformId = reader.GetInt();
            PlayerId = reader.GetInt();
        }
    }
    
    public struct NetworkTransformsForVehicle : INetSerializable
    {
        public byte HeaderByte;
        public int PlayerId;
        public int[] NetworkTransformIds;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(HeaderByte);
            writer.Put(PlayerId);
            writer.PutArray(NetworkTransformIds);
        }

        public void Deserialize(NetDataReader reader)
        {
            PlayerId = reader.GetInt();
            NetworkTransformIds = reader.GetIntArray();
        }
    }
    
    public struct GiveControlOfVehicleToClient : INetSerializable
    {
        public byte HeaderByte;
        public int PlayerId;

        public GiveControlOfVehicleToClient(int playerId)
        {
            HeaderByte = HeaderBytes.GiveControlOfVehicleToClient;
            PlayerId = playerId;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(HeaderByte);
            writer.Put(PlayerId);
        }

        public void Deserialize(NetDataReader reader)
        {
            PlayerId = reader.GetInt();
        }
    }
    
    public struct RemoveVehicle : INetSerializable
    {
        public byte headerByte;
        public int playerId;

        public RemoveVehicle(int playerId)
        {
            headerByte = HeaderBytes.RemoveVehicle;
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
    
    public struct VehicleEntityData : INetSerializable
    {
        public void Serialize(NetDataWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(NetDataReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}
