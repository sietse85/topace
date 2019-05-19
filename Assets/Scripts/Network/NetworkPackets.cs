using System.Numerics;
using LiteNetLib.Utils;
using UnityEngine;

namespace Network
{
    
    public struct RequestSpawn : INetSerializable
    {
        private byte headerByte;
        public int playerId;
        public int vehicleId;
        public byte[] config;

        public RequestSpawn(int playerId, int vehicleId, byte[] config)
        {
            headerByte = HeaderBytes.RequestSpawn;
            this.playerId = playerId;
            this.vehicleId = vehicleId;
            this.config = config;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(headerByte);
            writer.Put(playerId);
            writer.Put(vehicleId);
            writer.Put(config);
        }

        public void Deserialize(NetDataReader reader)
        {
            playerId = reader.GetInt();
            vehicleId = reader.GetInt();
            config = reader.GetRemainingBytes();
        }
    }

    public struct SpawnShip : INetSerializable
    {
        public byte headerByte;
        public int playerId;
        public int vehicleId;
        public float posx;
        public float posy;
        public float posz;
        public float rotx;
        public float roty;
        public float rotz;
        public float rotw;
        public byte[] config;
        

        public SpawnShip(int playerId, int vehicleId, float posx, float posy, float posz, float rotx,
            float roty, float rotz, float rotw, byte[] config)
        {
            headerByte = 0x05;
            this.playerId = playerId;
            this.vehicleId = vehicleId;
            this.posx = posx;
            this.posy = posy;
            this.posz = posz;
            this.rotx = rotx;
            this.roty = roty;
            this.rotz = rotz;
            this.rotw = rotw;
            this.config = config;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(headerByte);
            writer.Put(playerId);
            writer.Put(vehicleId);
            writer.Put(posx);
            writer.Put(posy);
            writer.Put(posz);
            writer.Put(rotx);
            writer.Put(roty);
            writer.Put(rotz);
            writer.Put(rotw);
            writer.Put(config);
        }

        public void Deserialize(NetDataReader reader)
        {
            playerId = reader.GetInt();
            vehicleId = reader.GetInt();
            posx = reader.GetFloat();
            posy = reader.GetFloat();
            posz = reader.GetFloat();
            rotx = reader.GetFloat();
            roty = reader.GetFloat();
            rotz = reader.GetFloat();
            rotw = reader.GetFloat();
            config = reader.GetRemainingBytes();
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
    
    public struct NetworkTransformUpdate : INetSerializable
    {
        public byte headerByte;
        public float locX;
        public float locY;
        public float locZ;
        public float rotX;
        public float rotY;
        public float rotZ;
        public float rotW;
        public int networkTransformId;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(HeaderBytes.NetworkTransFormId);
            writer.Put(locX);
            writer.Put(locY);
            writer.Put(locZ);
            writer.Put(rotX);
            writer.Put(rotY);
            writer.Put(rotZ);
            writer.Put(rotW);
            writer.Put(networkTransformId);
        }

        public void Deserialize(NetDataReader reader)
        {
            locX = reader.GetFloat();
            locY = reader.GetFloat();
            locZ = reader.GetFloat();
            rotX = reader.GetFloat();
            rotY = reader.GetFloat();
            rotZ = reader.GetFloat();
            rotW = reader.GetFloat();
            networkTransformId = reader.GetInt();
        }
    }
    
    public struct NetworkTransformsForVehicle : INetSerializable
    {
        public byte headerByte;
        public int vehicleId;
        public int playerId;
        public int[] networkTransformIds;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(headerByte);
            writer.Put(vehicleId);
            writer.Put(playerId);
            writer.PutArray(networkTransformIds);
        }

        public void Deserialize(NetDataReader reader)
        {
            vehicleId = reader.GetInt();
            playerId = reader.GetInt();
            networkTransformIds = reader.GetIntArray();
        }
    }
}
