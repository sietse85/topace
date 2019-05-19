using System.Collections.Generic;
using LiteNetLib;
using UnityEngine;
using Network;
using UnityEngine.Networking.Types;

public class GameManager : MonoBehaviour
{
//    public Dictionary<int, GameObject> playerShips;
    public Dictionary<int, NetworkTransform> networkTransforms;
    public Dictionary<int, GameObject> vehicles;
    public Dictionary<int, NetPeer> players;
    public Dictionary<int, string> playerNames;
    private Terrain map;
    public Server server;
    public PlayerDataHandler playerDataHandler;
    public VehicleDataHandler shipDataHandler;
    public VehicleConstructor vc;

    public GameObject playerShip;

    public int shipId = 0;
    public int playerId = 0;
    public int networkTransformId = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        map = Instantiate(Resources.Load("maps/terrain"), Vector3.zero, Quaternion.identity) as Terrain;
        Debug.Log("Default Map Instantiated");
        networkTransforms = new Dictionary<int, NetworkTransform>();
        vehicles = new Dictionary<int, GameObject>();
        players = new Dictionary<int, NetPeer>();
        playerNames = new Dictionary<int, string>();
        server = GetComponent<Server>();
        playerDataHandler = gameObject.GetComponent<PlayerDataHandler>();
        shipDataHandler = gameObject.GetComponent<VehicleDataHandler>();
        vc = gameObject.AddComponent<VehicleConstructor>();
    }

    // Update is called once per frame
    public void HandleReceived(NetPeer peer, NetPacketReader r)
    {
        byte header = r.GetByte();

        switch (header)
        {
            case HeaderBytes.SendUserName:
                playerDataHandler.ReceivePlayerName(peer, r);
                break;
            case HeaderBytes.RequestSpawn:
                shipDataHandler.PlayerRequestedShipSpawn(peer, r);
                break;
            case HeaderBytes.NetworkTransFormId:
                shipDataHandler.UpdateVehicleTransform(r);
                break;
            default:
                break;
        } 
    }

    public void DebugVehicles()
    {
        Debug.Log(vehicles.Count);

        foreach (KeyValuePair<int, GameObject> obj in vehicles)
        {

            NetworkTransform[] t = obj.Value.GetComponentsInChildren<NetworkTransform>();

            foreach (NetworkTransform nt in t)
            {
                Debug.Log(nt.GetPlayerId());
                Debug.Log(nt.GetTransformId());
            }
        }
    }
}



