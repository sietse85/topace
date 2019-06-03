using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace Server
{
    public class GameServer : MonoBehaviour, INetEventListener, INetLogger
    {
        public static GameServer instance;
        
        private NetDataWriter _writer;
        private NetManager _server;
        public int maxPlayers = 64;
        public int port = 5000;
        public string ip = "127.0.0.1";
        public float ticksPerSecond;
        
        //send the networktransfrom  updates to client each ... seconds
        public float updateSpeed = 0.1f;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
            
            maxPlayers = 64;
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
                _server.UpdateTime = 50;
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
            for (int i = 0; i < GameManager.instance.players.Length; i++) {
                if (GameManager.instance.players[i].slotOccupied)
                {
                    GameManager.instance.players[i].peer.Send(_writer, DeliveryMethod.ReliableUnordered);
                }
            }
        }
        
        public void SendBytesToAll(byte[] buf, int length)
        {
            if (GameManager.instance == null)
            {
                return;
            }

            for (int i = 0; i < GameManager.instance.players.Length; i++) {
                if (GameManager.instance.players[i].slotOccupied)
                {
                    GameManager.instance.players[i].peer.Send(buf, 0, length, DeliveryMethod.ReliableUnordered);
                }
            }
        }

        public void OnPeerConnected(NetPeer peer)
        {
            GameManager.instance.playerDataHandler.HandleNewConnection(peer);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            GameManager.instance.playerDataHandler.RemovePlayer(peer);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Debug.Log(socketError.ToString());
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            GameManager.instance.HandleReceived(peer, reader);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
            UnconnectedMessageType messageType)
        {
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            GameManager.instance.playerDataHandler.UpdateLatencyOfPlayer(peer, latency);
            
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
