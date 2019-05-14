using System.Collections.Generic;
using LiteNetLib;
using UnityEngine;
using Network;

public class GameManager : MonoBehaviour
{
    public Dictionary<int, GameObject> playerShips;
    public Dictionary<int, NetPeer> players;
    public Dictionary<int, string> playerNames;
    private Terrain map;
    private Server server;
    public PlayerDataHandler playerDataHandler;
    public ShipDataHandler shipDataHandler;

    public GameObject playerShip;

    public int shipId = 0;
    public int playerId = 0;
    
    
    // Start is called before the first frame update
    void Start()
    {
        map = Instantiate(Resources.Load("maps/terrain"), Vector3.zero, Quaternion.identity) as Terrain;
        Debug.Log("Default Map Instantiated");
        playerShips = new Dictionary<int, GameObject>();
        players = new Dictionary<int, NetPeer>();
        playerNames = new Dictionary<int, string>();
        server = FindObjectOfType<Server>();
        playerDataHandler = gameObject.AddComponent<PlayerDataHandler>();
        shipDataHandler = gameObject.AddComponent<ShipDataHandler>();
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
            default:
                break;
        } 
    }
}



