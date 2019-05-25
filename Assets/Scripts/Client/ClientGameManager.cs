using System.Collections.Generic;
using LiteNetLib;
using Menu;
using UnityEngine;
using Network;
using VehicleFunctions;

namespace Client
{
    public class ClientGameManager : MonoBehaviour
    {
        private GameClient _gameClient;
        public Dictionary<int, GameObject> _vehicles;
        private Dictionary<int, string> _playerNames;
        public Transform multiplayerMenu;
        public Transform spawnMenu;
        public Transform mainMenu;
        public Dictionary<int, Transform> networkTransforms;
        public VehicleConstructor vc;
        public int playerId;
        public ClientPlayerDataHandler playerDataHandler;
        public ClientVehicleDataHandler vehicleDataHandler;
        public NetworkTransformHandler networkTransformHandler;
        public VehicleController vehicleController;

        private void Start()
        {
            _gameClient = GetComponent<GameClient>();
            playerDataHandler = gameObject.GetComponent<ClientPlayerDataHandler>();
            vehicleDataHandler = gameObject.GetComponent<ClientVehicleDataHandler>();
            networkTransformHandler = gameObject.GetComponent<NetworkTransformHandler>();
            vehicleController = gameObject.GetComponent<VehicleController>();
            _vehicles = new Dictionary<int, GameObject>();
            networkTransforms = new Dictionary<int, Transform>();
            vc = gameObject.GetComponent<VehicleConstructor>();
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
                    playerDataHandler.SetPlayerId(r.GetInt());
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
            }
        }

        public void OpenSpawnMenu()
        {
            Debug.Log("open spawn menu");
            multiplayerMenu.GetComponentInChildren<Canvas>().enabled = false;
            mainMenu.GetComponentInChildren<Canvas>().enabled = false;
            spawnMenu.GetComponentInChildren<Canvas>().enabled = true;
            spawnMenu.GetComponentInChildren<ShipSelector>().LoadShipList();
        }

        public void ShowVehicleUI()
        {
            multiplayerMenu.GetComponentInChildren<Canvas>().enabled = false;
            mainMenu.GetComponentInChildren<Canvas>().enabled = false;
            spawnMenu.GetComponentInChildren<Canvas>().enabled = false;
        }
    }
}
