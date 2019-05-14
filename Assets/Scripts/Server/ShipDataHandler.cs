using System;
using LiteNetLib;
using UnityEngine;

namespace Network
{
    public class ShipDataHandler : MonoBehaviour
    {
        private Server _server;
        private GameManager _game;

        private void Start()
        {
            _server = GetComponent<Server>();
            _game = GetComponent<GameManager>();
        }

        public void PlayerRequestedShipSpawn(NetPeer peer, NetPacketReader r)
        {
            RequestSpawn packet = new RequestSpawn();
            packet.Deserialize(r);
            Byte[] b = new byte[1];
            b[0] = HeaderBytes.OpenSpawnMenuOnClient;
            peer.Send(b, DeliveryMethod.ReliableUnordered);
        }

        public void SpawnShip(int prefabId, int playerId)
        {
               
            
        }
    }
}