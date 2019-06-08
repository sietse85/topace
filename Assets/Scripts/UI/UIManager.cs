using System;
using Client;
using Menu;
using Resource;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {

        public static UIManager instance;
        
        public Transform mainMenu;
        public Transform multiplayerMenu;
        public Transform spawnMenu;
        public GameObject currentVehicleUI;

        private TextMeshProUGUI _altitude;
        private TextMeshProUGUI _velocity;
        public bool monitor;
        private RectTransform groundLevelIndicator;
        private Vector3 groundLeveLRotate;

        private byte VehicleId;

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
        }

        private void Start()
        {
            groundLeveLRotate = new Vector3();
        }

        private void Update()
        {
            if (monitor)
            {
                GameObject obj = ClientGameManager.instance.vehicleEntities[VehicleId].obj;
                _altitude.text = obj.transform.position.y.ToString("#0.0") + " m";
                groundLeveLRotate.z = obj.gameObject.transform.eulerAngles.z;
                groundLevelIndicator.rotation = Quaternion.Euler(groundLeveLRotate);
            }
        }

        public void SetVehicleIdToMonitorInUI(byte vehicleId)
        {
            VehicleId = vehicleId;
            monitor = true;
        }


        public void OpenSpawnMenu()
        {
            multiplayerMenu.GetComponentInChildren<Canvas>().enabled = false;
            mainMenu.GetComponentInChildren<Canvas>().enabled = false;
            spawnMenu.GetComponentInChildren<Canvas>().enabled = true;
            spawnMenu.GetComponentInChildren<VehicleSelector>().LoadShipList();
        }

        public void ShowVehicleUI(int vehicleDatabaseId)
        {
            multiplayerMenu.GetComponentInChildren<Canvas>().enabled = false;
            mainMenu.GetComponentInChildren<Canvas>().enabled = false;
            spawnMenu.GetComponentInChildren<Canvas>().enabled = false;

            if (currentVehicleUI != null)
            {
                Destroy(currentVehicleUI);
            }
            
            currentVehicleUI = Instantiate(Loader.instance.vehicles[vehicleDatabaseId].vehicleUI);
            InitVehicleUI();
        }

        public void InitVehicleUI()
        {
            _altitude = currentVehicleUI.transform.Find("Altitude").GetComponentInChildren<TextMeshProUGUI>();
            _velocity = currentVehicleUI.transform.Find("Velocity").GetComponentInChildren<TextMeshProUGUI>();
            groundLevelIndicator = currentVehicleUI.transform.Find("GroundLevelIndicator").GetComponent<RectTransform>();
        }
    }
}
