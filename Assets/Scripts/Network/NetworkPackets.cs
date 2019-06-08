using LiteNetLib.Utils;

namespace Network
{
    
    //client to server
    public struct RequestSpawn : INetSerializable
    {
        public readonly byte HeaderByte;
        public byte PlayerId;
        public int PlayerPin;
        public int VehicleDatabaseId;
        public byte[] Config;

        public RequestSpawn(byte playerId, int playerPin, int vehicleDatabaseId, byte[] config)
        {
            HeaderByte = HeaderBytes.RequestSpawn;
            PlayerId = playerId;
            PlayerPin = playerPin;
            VehicleDatabaseId = vehicleDatabaseId;
            Config = config;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(HeaderByte);
            writer.Put(PlayerId);
            writer.Put(PlayerPin);
            writer.Put(VehicleDatabaseId);
            writer.Put(Config);
        }

        public void Deserialize(NetDataReader reader)
        {
            PlayerId = reader.GetByte();
            PlayerPin = reader.GetInt();
            VehicleDatabaseId = reader.GetInt();
            Config = reader.GetRemainingBytes();
        }
    }

    //server to client
    public struct SpawnVehicle : INetSerializable
    {
        public byte HeaderByte;
        public byte PlayerId;
        public int VehicleDatabaseId;
        public float PosX;
        public float PosY;
        public float PosZ;
        public float RotX;
        public float RotY;
        public float RotZ;
        public float RotW;
        public byte[] Config;
        

        public SpawnVehicle(byte playerId, int vehicleDatabaseId, float posX, float posY, float posZ, float rotX,
            float rotY, float rotZ, float rotW, byte[] config)
        {
            HeaderByte = HeaderBytes.SpawnVehicle;
            PlayerId = playerId;
            VehicleDatabaseId = vehicleDatabaseId;
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
            RotX = rotX;
            RotY = rotY;
            RotZ = rotZ;
            RotW = rotW;
            Config = config;
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
            PlayerId = reader.GetByte();
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
    
    //server to client
    public struct AskClientForUsername : INetSerializable
    {
        public readonly byte HeaderByte;

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
    
    //client to server
    public struct SendUserNameToServer : INetSerializable
    {
        public readonly byte HeaderByte;
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
    
    //server to client, security pin send
    public struct SendPlayerId : INetSerializable
    {
        public byte HeaderByte;
        public byte PlayerId;
        public int PlayerPin;

        public SendPlayerId(byte playerId, int playerPin)
        {
            HeaderByte = 0x04;
            this.PlayerId = playerId;
            this.PlayerPin = playerPin;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(HeaderByte);
            writer.Put(PlayerId);
            writer.Put(PlayerPin);
        }

        public void Deserialize(NetDataReader reader)
        {
            HeaderByte = reader.GetByte();
            PlayerId = reader.GetByte();
            PlayerPin = reader.GetInt();
        }
    }
    
    //server to client
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
        public byte PlayerId;
        public int PlayerPin;
        
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
            writer.Put(PlayerPin);
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
            PlayerId = reader.GetByte();
            PlayerPin = reader.GetInt();
        }
    }
    
    //server to client
    public struct NetworkTransformsForVehicle : INetSerializable
    {
        public byte HeaderByte;
        public byte PlayerId;
        public int[] NetworkTransformIds;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(HeaderByte);
            writer.Put(PlayerId);
            writer.PutArray(NetworkTransformIds);
        }

        public void Deserialize(NetDataReader reader)
        {
            PlayerId = reader.GetByte();
            NetworkTransformIds = reader.GetIntArray();
        }
    }
    
    //server to client
    public struct GiveControlOfVehicleToClient : INetSerializable
    {
        public byte HeaderByte;
        public byte PlayerId;

        public GiveControlOfVehicleToClient(byte playerId)
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
            PlayerId = reader.GetByte();
        }
    }
    
    //server to client
    public struct RemoveVehicle : INetSerializable
    {
        public byte HeaderByte;
        public byte PlayerId;

        public RemoveVehicle(byte playerId)
        {
            HeaderByte = HeaderBytes.RemoveVehicle;
            PlayerId = playerId;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(HeaderByte);
            writer.Put(PlayerId);
        }

        public void Deserialize(NetDataReader reader)
        {
            PlayerId = reader.GetByte();
        }
    }
    
    //client to server
    public struct FireWeapon : INetSerializable
    {
        public byte HeaderByte;
        public byte PlayerId;
        public int PlayerPin;
        public int ProjectileDatabaseId;
        public byte WeaponSlotFired;
        public byte UniqueProjectileId;
        public float RotX;
        public float RotY;
        public float RotZ;
        public float RotW;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(HeaderByte);
            writer.Put(PlayerId);
            writer.Put(PlayerPin);
            writer.Put(ProjectileDatabaseId);
            writer.Put(WeaponSlotFired);
            writer.Put(UniqueProjectileId);
            writer.Put(RotX);
            writer.Put(RotY);
            writer.Put(RotZ);
            writer.Put(RotW);
        }

        public void Deserialize(NetDataReader reader)
        {
            PlayerId = reader.GetByte();
            PlayerPin = reader.GetInt();
            ProjectileDatabaseId = reader.GetInt();
            WeaponSlotFired = reader.GetByte();
            UniqueProjectileId = reader.GetByte();
            RotX = reader.GetFloat();
            RotY = reader.GetFloat();
            RotZ = reader.GetFloat();
            RotW = reader.GetFloat();
        }
    }
    
    public struct ReportCollision : INetSerializable
    {
        public byte HeaderByte;
        public byte PlayerIdThatWasHit;
        public int PlayerPin;
        public byte PlayerIdThatShotThisProjectile;
        public int ByProjectileDatabaseId;
        public byte UniqueProjectileId;
        public byte TickWhenCollisionOccurred;
        public float impactPosX;
        public float impactPosY;
        public float impactPosZ;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(HeaderByte);
            writer.Put(PlayerIdThatWasHit);
            writer.Put(PlayerPin);
            writer.Put(PlayerIdThatShotThisProjectile);
            writer.Put(ByProjectileDatabaseId);
            writer.Put(UniqueProjectileId);
            writer.Put(TickWhenCollisionOccurred);
            writer.Put(impactPosX);
            writer.Put(impactPosY);
            writer.Put(impactPosZ);
        }

        public void Deserialize(NetDataReader reader)
        {
            PlayerIdThatWasHit = reader.GetByte();
            PlayerPin = reader.GetInt();
            PlayerIdThatShotThisProjectile = reader.GetByte();
            ByProjectileDatabaseId = reader.GetInt();
            UniqueProjectileId = reader.GetByte();
            TickWhenCollisionOccurred = reader.GetByte();
            impactPosX = reader.GetFloat();
            impactPosY = reader.GetFloat();
            impactPosZ = reader.GetFloat();
        }
    }
}
