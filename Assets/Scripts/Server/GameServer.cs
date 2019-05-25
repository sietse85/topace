using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace Server
{
    public class Server : MonoBehaviour, INetEventListener, INetLogger
    {
        private NetDataWriter _writer;
        private NetManager _server;
        private GameManager _game;
        
        public int port = 5000;
        public string ip = "127.0.0.1";
        
        //send the networktransfrom  updates to client each ... seconds
        public float updateRatePerSecond = 0.033f;
        
        private void Start()
        {
            _game = GetComponent<GameManager>();
            NetDebug.Logger = this;
            _writer = new NetDataWriter();
            _server = new NetManager(this);
            bool started = _server.Start(port);

            if (!started)
            {
                Debug.Log("Server could not be started!");
            }
            else
            {
                Debug.Log("Server Started");
                _server.BroadcastReceiveEnabled = true;
                _server.UpdateTime = 25;
            }
        }

        private void Update()
        {
            _server.PollEvents();
        }

        public void Send<T>(T packet, NetPeer peer) where T : struct, INetSerializable
        {
            _writer.Reset();
            packet.Serialize(_writer);
            peer.Send(_writer, DeliveryMethod.ReliableUnordered);
        }
        
        public void SendToAll<T>(T packet) where T : struct, INetSerializable
        {
            _writer.Reset();
            packet.Serialize(_writer);
            foreach (KeyValuePair<int, NetPeer> p in _game.players)
                _game.players[p.Key].Send(_writer, DeliveryMethod.ReliableUnordered);
        }

        public void OnPeerConnected(NetPeer peer)
        {
            _game.playerDataHandler.HandleNewConnection(peer);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            _game.playerDataHandler.RemovePlayer(peer);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Debug.Log(socketError.ToString());
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            _game.HandleReceived(peer, reader);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
            UnconnectedMessageType messageType)
        {
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            Debug.Log("Connection requested");
            request.AcceptIfKey("topace");
        }

        public void WriteNet(NetLogLevel level, string str, params object[] args)
        {
        }

        private void OnDestroy()
        {
            NetDebug.Logger = null;
            if (_server != null)
            {
                _server.Stop();
            }
        }
        
    }
}
