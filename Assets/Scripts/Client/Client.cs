using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace Network
{
    public class Client : MonoBehaviour, INetEventListener
    {
        private NetManager _client;
        private ClientGameManager _game;
        private NetDataWriter _writer;
        private NetPeer _server;

        // Start is called before the first frame update
        void Start()
        {
            _game = GetComponent<ClientGameManager>();
            _writer = new NetDataWriter();
            _client = new NetManager(this);
            _client.UnconnectedMessagesEnabled = true;
            _client.UpdateTime = 25;
            if (_client.Start())
            {
                Debug.Log("Client started");
            }
        }

        public void Connect()
        {
            _client.Connect("127.0.0.1", 5000, "topace");
        }

        // Update is called once per frame
        void Update()
        {
            _client.PollEvents();
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log(("[C] connected " + peer.EndPoint));
            _server = peer;
        }


        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Debug.Log("[C] " + socketError);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            _game.HandleReceived(reader);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
            UnconnectedMessageType messageType)
        {
            if (messageType == UnconnectedMessageType.BasicMessage && _client.PeersCount == 0 && reader.GetInt() == 1)
            {
                Debug.Log("[CLIENT] Received discovery response. Connecting to: " + remoteEndPoint);
                _client.Connect(remoteEndPoint, "sample_app");
            }
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {

        }

        public void OnConnectionRequest(ConnectionRequest request)
        {

        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log("[C] We disconnected because " + disconnectInfo.Reason);
        }
        
        public void Send<T>(T packet) where T : struct, INetSerializable
        {
            _writer.Reset();
            packet.Serialize(_writer);
            _server.Send(_writer, DeliveryMethod.ReliableSequenced);
        }
    }
}
