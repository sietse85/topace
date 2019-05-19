using System.Collections.Generic;
using LiteNetLib;
using UnityEngine;
using Network;

public class ClientGameManager : MonoBehaviour
{
    private Client _client;
    
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
    
    private void Start()
    {
        _client = GetComponent<Client>();
        playerDataHandler = gameObject.GetComponent<ClientPlayerDataHandler>();
        vehicleDataHandler = gameObject.GetComponent<ClientVehicleDataHandler>();
        networkTransformHandler = gameObject.GetComponent<NetworkTransformHandler>();
        _vehicles = new Dictionary<int, GameObject>();
        
        networkTransforms = new Dictionary<int, Transform>();
        vc = gameObject.GetComponent<VehicleConstructor>();
    }

    public void HandleReceived(NetPacketReader r)
    {
        byte header = r.GetByte();

        switch (header)
        {
            case HeaderBytes.AskForUserName:
                playerDataHandler.SendPlayerName();
                break;
            case HeaderBytes.SendPlayerId:
                Debug.Log("player id packet recevied");
                playerDataHandler.SetPlayerId(r.GetInt());
                vehicleDataHandler.TestSpawn();
                break;
            case HeaderBytes.OpenSpawnMenuOnClient:
                OpenSpawnMenu();
                break;
            case HeaderBytes.SpawnShipOnClient:
                vc.ConstructVehicleFromPacket(r);
                break;
            case HeaderBytes.NetworkTransFormId:
                networkTransformHandler.UpdateNetworkTransform(r);
                break;
            case HeaderBytes.NetworkTransFormsForVehicle:
                Debug.Log("transforms received");
                networkTransformHandler.SetTransformIds(r);
                break;
            default:
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
}
