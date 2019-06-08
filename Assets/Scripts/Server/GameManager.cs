using System;
using System.Collections.Generic;
using LiteNetLib;
using UnityEngine;
using Network;
using Vehicle;

namespace Server
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        
        public Player[] players;
        public VehicleEntity[] vehicleEntities;
        public ProjectileReference[] projectiles; 
        public Dictionary<int, string> playerNames;
        public Dictionary<int, TurretSlot[]> turrets;
        public PlayerDataHandler playerDataHandler;
        public VehicleDataHandler vehicleDataHandler;
        public VehicleConstructor vc;
        public Ticker ticker;
        public int networkTransformId;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }


        }

        private void Start()
        {
            turrets = new Dictionary<int, TurretSlot[]>(GameServer.instance.maxPlayers);
            players = new Player[GameServer.instance.maxPlayers];
            playerNames = new Dictionary<int, string>(GameServer.instance.maxPlayers);
            projectiles = new ProjectileReference[GameServer.instance.maxPlayers * 100];
            vehicleEntities = new VehicleEntity[GameServer.instance.maxPlayers];
            playerDataHandler = gameObject.GetComponent<PlayerDataHandler>();
            vehicleDataHandler = gameObject.GetComponent<VehicleDataHandler>();
            vc = gameObject.GetComponent<VehicleConstructor>();
            ticker = GetComponent<Ticker>();
        }

        // Update is called once per frame
        public void HandleReceived(NetPeer peer, NetPacketReader r)
        {
            byte header = r.GetByte();
            switch (header)
            {
                case HeaderBytes.SendUserNameToServer:
                    playerDataHandler.ReceivePlayerName(peer, r);
                    break;
                case HeaderBytes.RequestSpawn:
                    vehicleDataHandler.ClientRequestedVehicleSpawn(peer, r);
                    break;
                case HeaderBytes.NetworkTransFormId:
                    vehicleDataHandler.UpdateVehicleTransform(r);
                    break;
                case HeaderBytes.FireWeapon:
                    ticker.AddFireCommand(r);
                    break;
                case HeaderBytes.ReportCollision:
                    ticker.AddCollisionReport(r);
                    break;
            }
        }
    }
}
