using System;
using System.Collections;
using System.Diagnostics;
using LiteNetLib;
using Network;
using UnityEngine;
using VehicleFunctions;
using Debug = UnityEngine.Debug;

namespace Server
{
    public class Ticker : MonoBehaviour
    {
        //buffers to fill with various data
        private byte[] networkTransformData;
        private byte[] vehicleData;
        private byte[] playerData;
        private byte[] fireBuf;
        
        public NetworkTransformStruct[] networkTransforms;

        public GameServer gameServer;
        public GameManager game;
        private ByteHelper b;
        private int sizePacket = 507;
        private FireCommand[] fireCommands;
        private FireWeapon firePacket;
        private int fireCommandIndex = 0;
        private Stopwatch s;
        
        private void Start()
        {
            game = GetComponent<GameManager>();
            gameServer = GetComponent<GameServer>();
            b = gameObject.AddComponent<ByteHelper>();
            
            networkTransformData = new byte[sizePacket];
            vehicleData = new byte[sizePacket];
            playerData = new byte[sizePacket];
            fireBuf = new byte[sizePacket];
            
            firePacket = new FireWeapon();
            fireCommands = new FireCommand[512];
            s = new Stopwatch();
            
            networkTransforms = new NetworkTransformStruct[1024];
            gameServer.updateSpeed = 1f / gameServer.ticksPerSecond;
            StartCoroutine(Tick());

        }

        public IEnumerator Tick()
        {
            while (true)
            {
                Debug.Log("Ticking");
                s.Start();
                s.Restart();
                
                SendVehicleInformationToClients();
                ProcessFireCommands();
                int i = SendNetworkTransformsToClient();
                SendPlayerInformationToClients();
                
                s.Stop();
                
//                if (game.players != null)
//                    Debug.Log("The tick took " + s.ElapsedMilliseconds + " ms to process");

                yield return new WaitForSeconds(gameServer.updateSpeed);
            }
        }

        private void ProcessFireCommands()
        {
            fireCommandIndex = 0;
            int index = 0;
            fireBuf[0] = HeaderBytes.FireWeapon;
            for (int i = 0; i < fireCommands.Length; i++)
            {
                if (!fireCommands[i].process)
                {
                    return;
                }
                
                fireCommands[i].process = false;

                if (!game.turrets[fireCommands[i].vehicleId][fireCommands[i].weaponSlotFired].inCoolDown)
                {
                    //fire the turret on the server itself
                    game.turrets[fireCommands[i].vehicleId][fireCommands[i].weaponSlotFired].Fire();
                }
            }
        }

        private int SendNetworkTransformsToClient()
        {
            int index = 0;
            networkTransformData[index] = HeaderBytes.NetworkTransFormId;
            index++;
            int iterations = 0;
            for(int i = 0; i < networkTransforms.Length; i++)
            {
                if (!networkTransforms[i].processInTick)
                    continue;
                Buffer.BlockCopy(b.Vector3ToByte(networkTransforms[i].transform.position), 0, networkTransformData, index, sizeof(float) * 3);
                index += sizeof(float) * 3;
                Buffer.BlockCopy(b.QuaternionToByte(networkTransforms[i].transform.rotation), 0, networkTransformData, index, sizeof(float) * 4);
                index += sizeof(float) * 4;
                Buffer.BlockCopy(b.IntToByte(i), 0, networkTransformData, index, sizeof(int));
                index += sizeof(int);
                networkTransformData[index] = networkTransforms[i].playerId;
                index += sizeof(byte);

                //no room left in buffer, send and go on
                if (sizePacket - index < 34)
                {
                    gameServer.SendBytesToAll(networkTransformData, index);
                    ClearBuf(ref networkTransformData);
                    index = 0;
                    networkTransformData[index] = HeaderBytes.NetworkTransFormId;
                    index++;
                }
                
                iterations++;
            }
            
            gameServer.SendBytesToAll(networkTransformData, index);
            ClearBuf(ref networkTransformData);

            return iterations;
        }

        public void SendVehicleInformationToClients()
        {
            Debug.Log("Sending vheicle info");
            int index = 0;
            vehicleData[index] = HeaderBytes.SendVehicleData;
            index += sizeof(byte);
            
            for (int i = 0; i < game.vehicleEntities.Length; i++)
            {
                VehicleEntity entity = game.vehicleEntities[i];
                if (!entity.processInTick)
                    continue;

                vehicleData[index] = (byte) i;
                index += sizeof(byte);
                Buffer.BlockCopy(b.FloatToByte(entity.currentArmor), 0, vehicleData, index, sizeof(float));
                index += sizeof(float);
                Buffer.BlockCopy(b.FloatToByte(entity.currentHealth), 0, vehicleData, index, sizeof(float));
                index += sizeof(float);
                Buffer.BlockCopy(b.FloatToByte(entity.currentShield), 0, vehicleData, index, sizeof(float));
                index += sizeof(float);
                Buffer.BlockCopy(b.FloatToByte(entity.battery), 0, vehicleData, index, sizeof(float));
                index += sizeof(float);

                if (sizePacket - index < 18)
                {
                    gameServer.SendBytesToAll(vehicleData, index);
                    ClearBuf(ref vehicleData);
                    index = 0;
                    vehicleData[0] = HeaderBytes.SendVehicleData;
                    index += sizeof(byte);
                }
                
                gameServer.SendBytesToAll(vehicleData, index);
                ClearBuf(ref vehicleData);
            }
        }

        public void SendPlayerInformationToClients()
        {
            if (game == null)
            {
                Debug.Log("Gamemanager null");
                return;
            }

            for (int i = 0; i < game.players.Length; i++)
            {
                
                if(!game.players[i].processInTick)
                    continue;
                
                int index = 0;
                playerData[index] = HeaderBytes.SendPlayerData;
                index++;
                    
                Buffer.BlockCopy(b.IntToByte(i), 0, playerData, index, sizeof(int));
                index += sizeof(int);
                Buffer.BlockCopy(b.IntToByte(game.players[i].latency), 0, playerData, index, sizeof(int));
                index += sizeof(int);
                Buffer.BlockCopy(b.IntToByte(game.players[i].score), 0, playerData, index, sizeof(int));
                index += sizeof(int);
                Buffer.BlockCopy(b.IntToByte(game.players[i].kills), 0, playerData, index, sizeof(int));
                index += sizeof(int);
                Buffer.BlockCopy(b.IntToByte(game.players[i].deaths), 0, playerData, index, sizeof(int));
                index += sizeof(int);
                Buffer.BlockCopy(b.IntToByte(game.players[i].shotsFired), 0, playerData, index, sizeof(int));
                index += sizeof(int);
                Buffer.BlockCopy(b.IntToByte(game.players[i].shotsHit), 0, playerData, index, sizeof(int));
                index += sizeof(int);
                
                game.gameServer.SendBytesToAll(playerData, index); 
                ClearBuf(ref playerData);
            }
        }

        public void ClearBuf(ref byte[] buf)
        {
            foreach (byte b in buf)
            {
                buf[b] = 0x00;
            }
        }

        public void AddFireCommand(NetPacketReader r)
        {
            firePacket.Deserialize(r);
            if (firePacket.playerPin != game.players[firePacket.playerId].securityPin)
                return;
            fireCommands[fireCommandIndex].projectileId = firePacket.projectileId;
            fireCommands[fireCommandIndex].vehicleId = firePacket.playerId;
            fireCommands[fireCommandIndex].weaponSlotFired = firePacket.weaponSlotFired;
            fireCommands[fireCommandIndex].bulletCount = firePacket.bulletId;
            fireCommands[fireCommandIndex].process = true;
            fireCommandIndex++;
        }
    }
}