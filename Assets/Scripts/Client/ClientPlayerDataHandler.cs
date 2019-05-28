using System;
using System.Text;
using LiteNetLib;
using Network;
using Server;
using UnityEngine;

namespace Client
{
    public class ClientPlayerDataHandler : MonoBehaviour
    {
        private GameClient _gameClient;
        private ClientGameManager _game;

        private ByteHelper b;
        private byte[] intBuf;

        private void Start()
        {
            _gameClient = GetComponent<GameClient>();
            _game = GetComponent<ClientGameManager>();
            b = gameObject.AddComponent<ByteHelper>();
            intBuf = new byte[4];
        }
         
        public void SendPlayerName()
        {
            SendUserNameToServer packet = new SendUserNameToServer("Sietse");
            _gameClient.Send(packet); 
        }

        public void SetPlayerId(int playerId)
        {
            Debug.Log("Client playerId = " + playerId);
            _game.playerId = playerId;
        }

        public void UpdatePlayerData(NetPacketReader r)
        {
            byte[] buf = r.GetRemainingBytes();
            int index = 0;
            Buffer.BlockCopy(buf, index, intBuf, 0, sizeof(int));
            int playerId = b.ByteToInt(intBuf);
            index += sizeof(int);
            Buffer.BlockCopy(buf, index, intBuf, 0, sizeof(int));
            int latency = b.ByteToInt(intBuf);
            index += sizeof(int);
            Buffer.BlockCopy(buf, index, intBuf, 0, sizeof(int));
            int score = b.ByteToInt(intBuf);
            index += sizeof(int);
            Buffer.BlockCopy(buf, index, intBuf, 0, sizeof(int));
            int kills = b.ByteToInt(intBuf);
            index += sizeof(int);
            Buffer.BlockCopy(buf, index, intBuf, 0, sizeof(int));
            int deaths = b.ByteToInt(intBuf);
            index += sizeof(int);
            Buffer.BlockCopy(buf, index, intBuf, 0, sizeof(int));
            int shotsFired = b.ByteToInt(intBuf);
            index += sizeof(int);
            Buffer.BlockCopy(buf, index, intBuf, 0, sizeof(int));
            int shotsHit = b.ByteToInt(intBuf);

            _game.players[playerId].latency = latency;
            _game.players[playerId].score = score;
            _game.players[playerId].kills = kills;
            _game.players[playerId].deaths = deaths;
            _game.players[playerId].shotsFired = shotsFired;
            _game.players[playerId].shotsHit = shotsHit;
        }
    }
}