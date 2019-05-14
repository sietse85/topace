using System.Collections.Generic;
using LiteNetLib;
using UnityEngine;

namespace Network
{
    public class PlayerDataHandler : MonoBehaviour
    {

        private Server server;
        private GameManager game;

        private void Start()
        {
            server = GetComponent<Server>();
            game = GetComponent<GameManager>();
        }

        public void ReceivePlayerName(NetPeer peer, NetPacketReader r)
        {
            game.playerId++;
            SendUserName n = new SendUserName();
            n.Deserialize(r);
            game.players.Add(game.playerId, peer);
            game.playerNames.Add(game.playerId, n.playerName);
            SendPlayerId(peer, game.playerId);
        }

        public void SendPlayerId(NetPeer peer, int playerId)
        {
            SendPlayerId packet = new SendPlayerId(playerId);
            server.Send(packet, peer);
        }

        public void RemovePlayer(NetPeer peer)
        {
            foreach (KeyValuePair<int, NetPeer> p in game.players)
            {
                if (p.Value == peer)
                {
                    game.players.Remove(p.Key);
                    game.playerNames.Remove(p.Key);
                }
            }
        }

        public void AskForUserName(NetPeer peer)
        {
            AskForUserName askname = new AskForUserName(0x01);
            server.Send(askname, peer);
        }
    }
}