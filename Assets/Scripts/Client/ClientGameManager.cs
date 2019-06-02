using System;
using System.Collections.Generic;
using LiteNetLib;
using Menu;
using UnityEngine;
using Network;
using Server;
using VehicleFunctions;

namespace Client
{
    public class ClientGameManager : MonoBehaviour
    {
        public GameClient client;
        public NetworkTransformStruct[] networkTransforms;
        public VehicleEntity[] vehicleEntities;
        public Player[] players;
        public ProjectileReference[] projectiles; 
        private Dictionary<int, string> _playerNames;
        public Transform multiplayerMenu;
        public Transform spawnMenu;
        public Transform mainMenu;
        public VehicleConstructor vc;
        public byte playerId;
        public ClientPlayerDataHandler playerDataHandler;
        public ClientVehicleDataHandler vehicleDataHandler;
        public NetworkTransformHandler networkTransformHandler;
        public VehicleController vehicleController;
        public int securityPin;
        public uint bulletId;

        private void Awake()
        {
            Debug.Log("A client manager awoke");
        }

        private void Start()
        {
            networkTransforms = new NetworkTransformStruct[1024];
            vehicleEntities = new VehicleEntity[254];
            projectiles = new ProjectileReference[254 * 100];
            client = GetComponent<GameClient>();
            playerDataHandler = gameObject.GetComponent<ClientPlayerDataHandler>();
            vehicleDataHandler = gameObject.GetComponent<ClientVehicleDataHandler>();
            networkTransformHandler = gameObject.GetComponent<NetworkTransformHandler>();
            vehicleController = gameObject.GetComponent<VehicleController>();
            vc = gameObject.GetComponent<VehicleConstructor>();
            players = new Player[64];
        }

        public void HandleReceived(NetPacketReader r)
        {
            byte header = r.GetByte();

            switch (header)
            {
                case HeaderBytes.AskClientForUsername:
                    playerDataHandler.SendPlayerName();
                    break;
                case HeaderBytes.SendPlayerId:
                    playerDataHandler.SetPlayerId(r);
                    vehicleDataHandler.TestSpawn();
                    break;
                case HeaderBytes.OpenSpawnMenuOnClient:
                    OpenSpawnMenu();
                    break;
                case HeaderBytes.SpawnVehicle:
                    vc.ConstructVehicleFromPacket(r);
                    break;
                case HeaderBytes.NetworkTransFormId:
                    networkTransformHandler.UpdateNetworkTransform(r);
                    break;
                case HeaderBytes.NetworkTransFormsForVehicle:
                    networkTransformHandler.SetTransformIds(r);
                    break;
                case HeaderBytes.RemoveVehicle:
                    vehicleDataHandler.RemoveVehicle(r);
                    break;
                case HeaderBytes.SendPlayerData:
                    playerDataHandler.UpdatePlayerData(r);
                    break;
                case HeaderBytes.SendVehicleData:
                    vehicleDataHandler.UpdateVehicleInfo(r);
                    break;
            }
        }

        public void OpenSpawnMenu()
        {
            Debug.Log("open spawn menu");
            multiplayerMenu.GetComponentInChildren<Canvas>().enabled = false;
            mainMenu.GetComponentInChildren<Canvas>().enabled = false;
            spawnMenu.GetComponentInChildren<Canvas>().enabled = true;
            spawnMenu.GetComponentInChildren<VehicleSelector>().LoadShipList();
        }

        public void ShowVehicleUI()
        {
            multiplayerMenu.GetComponentInChildren<Canvas>().enabled = false;
            mainMenu.GetComponentInChildren<Canvas>().enabled = false;
            spawnMenu.GetComponentInChildren<Canvas>().enabled = false;
        }
    }
}
