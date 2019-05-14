using System.Collections.Generic;
using LiteNetLib;
using UnityEngine;
using Network;

public class ClientGameManager : MonoBehaviour
{
    private Client _client;
    
    private Dictionary<int, GameObject> _playerShips;
    private Dictionary<int, string> _playerNames;
    public Transform multiplayerMenu;
    public Transform spawnMenu;
    public Transform mainMenu;

    public int playerId;

    public ClientPlayerDataHandler playerDataHandler;
    public ClientShipDataHandler shipDataHandler;
    
    private void Start()
    {
        _client = GetComponent<Client>();
        playerDataHandler = gameObject.AddComponent<ClientPlayerDataHandler>();
        shipDataHandler = gameObject.AddComponent<ClientShipDataHandler>();
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
                playerDataHandler.SetPlayerId(r.GetInt());
                break;
            case HeaderBytes.OpenSpawnMenuOnClient:
                OpenSpawnMenu();
                break;
                
            default:
                break;
        } 
    }

    public void OpenSpawnMenu()
    {
        multiplayerMenu.GetComponentInChildren<Canvas>().enabled = false;
        mainMenu.GetComponentInChildren<Canvas>().enabled = false;
        spawnMenu.GetComponentInChildren<Canvas>().enabled = true;
        spawnMenu.GetComponentInChildren<ShipSelector>().LoadShipList();
    }
}
