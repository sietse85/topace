using System;
using System.Collections;
using System.Diagnostics;
using LiteNetLib;
using Network;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Server
{
    public class Ticker : MonoBehaviour
    {
        private byte[] networkTransformData;
        private byte[] playerData;
        public NetworkTransformStruct[] networkTransforms;

        public GameServer gameServer;
        public GameManager game;
        private ByteHelper b;
        private int sizePacket = 507;
        private FireCommand[] fireCommands;
        private byte[] fireBuf;
        private FireWeapon firePacket;
        private int fireCommandIndex = 0;

        private Stopwatch s;
        private void Start()
        {
            game = GetComponent<GameManager>();
            gameServer = GetComponent<GameServer>();
            b = gameObject.AddComponent<ByteHelper>();
            networkTransformData = new byte[sizePacket];
            playerData = new byte[sizePacket];
            firePacket = new FireWeapon();
            fireCommands = new FireCommand[512];
            fireBuf = new byte[sizePacket];
            s = new Stopwatch();
            networkTransforms = new NetworkTransformStruct[1024];
            gameServer.updateSpeed = 1f / gameServer.ticksPerSecond;
            StartCoroutine(Tick());

        }

        public IEnumerator Tick()
        {
            while (true)
            {
                s.Start();
                s.Restart();
                
                ProcessFireCommands();
                int i = SendNetworkTransformsToClient();
                SendPlayerInformationToClients();
                
                s.Stop();
                
                if (game.players != null)
                    Debug.Log("The tick took " + s.ElapsedMilliseconds + " ms to process");

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
                Buffer.BlockCopy(b.Vector3ToByte(networkTransforms[i].position), 0, networkTransformData, index, sizeof(float) * 3);
                index += sizeof(float) * 3;
                Buffer.BlockCopy(b.QuaternionToByte(networkTransforms[i].rotation), 0, networkTransformData, index, sizeof(float) * 4);
                index += sizeof(float) * 4;
                Buffer.BlockCopy(b.IntToByte(networkTransforms[i].networkTransformId), 0, networkTransformData, index, sizeof(int));
                index += sizeof(int);
                Buffer.BlockCopy(b.IntToByte(networkTransforms[i].playerId), 0, networkTransformData, index, sizeof(int));
                index += sizeof(int);

                //no room left in buffer, send and go on
                if (sizePacket - index < 37)
                {
                    index = 0;
                    networkTransformData[index] = HeaderBytes.NetworkTransFormId;
                    index++;
                    gameServer.SendBytesToAll(networkTransformData);
                    ClearBuf(ref networkTransformData);
                }
                
                iterations++;
            }
            
            gameServer.SendBytesToAll(networkTransformData);
            ClearBuf(ref networkTransformData);

            return iterations;
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
                
                game.gameServer.SendBytesToAll(playerData); 
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
            Debug.Log("Received fire command");
            firePacket.Deserialize(r);
            fireCommands[fireCommandIndex].projectileId = firePacket.projectileId;
            fireCommands[fireCommandIndex].vehicleId = firePacket.vehicleId;
            fireCommands[fireCommandIndex].weaponSlotFired = firePacket.weaponSlotFired;
            fireCommands[fireCommandIndex].process = true;
            fireCommandIndex++;
        }
    }
}