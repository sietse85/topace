﻿using System;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace Client
{
    public class GameClient : MonoBehaviour, INetEventListener
    {
        public static GameClient instance;
        
        private NetManager _client;
        private NetDataWriter _writer;
        private NetPeer _server;
        public float updateSpeedNetworktransforms = 0.1f;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(instance);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            _writer = new NetDataWriter();
            _client = new NetManager(this);
            _client.UnconnectedMessagesEnabled = true;
            _client.UpdateTime = 25;
            _client.Start();
        }

        public void Connect()
        {
            if (_client.IsRunning)
            {
                _client.Connect("127.0.0.1", 5000, "topace");
            }
        }

        // Update is called once per frame
        void Update()
        {
            _client.PollEvents();
        }

        public void OnPeerConnected(NetPeer peer)
        {
            _server = peer;
        }


        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            ClientGameManager.instance.HandleReceived(reader);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
            UnconnectedMessageType messageType)
        {
            if (messageType == UnconnectedMessageType.BasicMessage && _client.PeersCount == 0 && reader.GetInt() == 1)
            {
                _client.Connect(remoteEndPoint, "topace");
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
        }
        
        public void Send<T>(T packet) where T : struct, INetSerializable
        {
            _writer.Reset();
            packet.Serialize(_writer);
            _server.Send(_writer, DeliveryMethod.Unreliable);
        }
    }
}
