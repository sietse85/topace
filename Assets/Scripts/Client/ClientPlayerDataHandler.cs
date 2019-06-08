using System;
using LiteNetLib;
using Network;
using Server;
using UnityEngine;

namespace Client
{
    public class ClientPlayerDataHandler : MonoBehaviour
    {
        private byte[] intBuf;

        private void Start()
        {
            intBuf = new byte[4];
        }
         
        public void SendPlayerName(string userName)
        {
            SendUserNameToServer packet = new SendUserNameToServer(userName);
            GameClient.instance.Send(packet); 
        }

        public void SetPlayerId(NetPacketReader r)
        {
            ClientGameManager.instance.playerId = r.GetByte();
            ClientGameManager.instance.securityPin = r.GetInt();
        }

        public void UpdatePlayerData(ref byte[] snapshot)
        {

            int index = ClientGameManager.instance.index;
            byte playerId = snapshot[index];
            index += sizeof(byte);
            Buffer.BlockCopy(snapshot, index, intBuf, 0, sizeof(int));
            int latency = ByteHelper.instance.ByteToInt(intBuf);
            index += sizeof(int);
            Buffer.BlockCopy(snapshot, index, intBuf, 0, sizeof(int));
            int score = ByteHelper.instance.ByteToInt(intBuf);
            index += sizeof(int);
            Buffer.BlockCopy(snapshot, index, intBuf, 0, sizeof(int));
            int kills = ByteHelper.instance.ByteToInt(intBuf);
            index += sizeof(int);
            Buffer.BlockCopy(snapshot, index, intBuf, 0, sizeof(int));
            int deaths = ByteHelper.instance.ByteToInt(intBuf);
            index += sizeof(int);
            Buffer.BlockCopy(snapshot, index, intBuf, 0, sizeof(int));
            int shotsFired = ByteHelper.instance.ByteToInt(intBuf);
            index += sizeof(int);
            Buffer.BlockCopy(snapshot, index, intBuf, 0, sizeof(int));
            int shotsHit = ByteHelper.instance.ByteToInt(intBuf);
            index += sizeof(int);

            ClientGameManager.instance.players[playerId].latency = latency;
            ClientGameManager.instance.players[playerId].score = score;
            ClientGameManager.instance.players[playerId].kills = kills;
            ClientGameManager.instance.players[playerId].deaths = deaths;
            ClientGameManager.instance.players[playerId].shotsFired = shotsFired;
            ClientGameManager.instance.players[playerId].shotsHit = shotsHit;

            ClientGameManager.instance.index = index;
        }
    }
}