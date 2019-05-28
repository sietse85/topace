using System.Collections.Generic;
using LiteNetLib;
using UnityEngine;
using Network;
using VehicleFunctions;

namespace Server
{
    public class GameManager : MonoBehaviour
    {
        public Player[] players;
        public Dictionary<int, string> playerNames;
        public bool[] playerSlots;
        public Dictionary<int, GameObject> vehicles;
        public VehicleEntity[] VehicleEntities;
        public GameServer gameServer;
        public PlayerDataHandler playerDataHandler;
        public VehicleDataHandler vehicleDataHandler;
        public VehicleConstructor vc;
        public Ticker ticker;
        public int networkTransformId;

        // Start is called before the first frame update
        void Awake()
        {
            gameServer = GetComponent<GameServer>();
            vehicles = new Dictionary<int, GameObject>();
            players = new Player[gameServer.maxPlayers];
            playerNames = new Dictionary<int, string>();
            VehicleEntities = new VehicleEntity[gameServer.maxPlayers];
            playerSlots = new bool[gameServer.maxPlayers];
            playerDataHandler = gameObject.GetComponent<PlayerDataHandler>();
            vehicleDataHandler = gameObject.GetComponent<VehicleDataHandler>();
            vc = gameObject.GetComponent<VehicleConstructor>();
            ticker = gameObject.GetComponent<Ticker>();
            if (ticker == null)
            {
                Debug.Log("Network transform handler could not be loaded");
            }
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
            }
        }
    }
}
