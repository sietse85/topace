using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Network;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Server
{
    public class Ticker : MonoBehaviour
    {
        private byte[] networkTransformData;
        private byte[] playerData;
        public Dictionary<int, NetworkTransform> networkTransforms;

        public GameServer gameServer;
        public GameManager game;
        private ByteHelper b;
        private int sizePacket = 507;

        private Stopwatch s;
        private void Start()
        {
            networkTransformData = new byte[sizePacket];
            playerData = new byte[sizePacket];
            s = new Stopwatch();
            gameServer = GetComponent<GameServer>();
            game = GetComponent<GameManager>();
            networkTransforms = new Dictionary<int, NetworkTransform>();
            StartCoroutine(Tick());
            b = gameObject.AddComponent<ByteHelper>();

        }

        public IEnumerator Tick()
        {
            while (true)
            {
                s.Start();
                s.Restart();

                int i = SendNetworkTransformsToClient();
                SendPlayerInformationToClients();
                
                s.Stop();
                
                if (game.players != null)
                    Debug.Log(i + " networkTransforms send to players in " + (s.ElapsedTicks / 10000f).ToString("#.0000000000") + " ms");

                yield return new WaitForSeconds(gameServer.updateSpeed);
            }
        }

        private int SendNetworkTransformsToClient()
        {
            int i = 0;
            int index = 0;
            foreach (KeyValuePair<int, NetworkTransform> t in networkTransforms)
            {
                networkTransformData[index] = HeaderBytes.NetworkTransFormId;
                index++;
                Buffer.BlockCopy(b.Vector3ToByte(t.Value.transform.position), 0, networkTransformData, index, sizeof(float) * 3);
                index += sizeof(float) * 3;
                Buffer.BlockCopy(b.QuaternionToByte(t.Value.transform.rotation), 0, networkTransformData, index, sizeof(float) * 4);
                index += sizeof(float) * 4;
                Buffer.BlockCopy(b.IntToByte(t.Value.GetTransformId()), 0, networkTransformData, index, sizeof(int));
                index += sizeof(int);
                Buffer.BlockCopy(b.IntToByte(t.Value.GetPlayerId()), 0, networkTransformData, index, sizeof(int));
                index += sizeof(int);

                if (sizePacket - index < 37)
                {
                    index = 0;
                    gameServer.SendBytesToAll(networkTransformData);
                    ClearBuf(ref networkTransformData);
                }
                
                i++;
            }
            
            gameServer.SendBytesToAll(networkTransformData);
            ClearBuf(ref networkTransformData);

            return i;
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
    }
}