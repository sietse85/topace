using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using Scriptable;
using UnityEngine;

public class VehicleConstructor : MonoBehaviour
{
    private GameManager _serverGame;
    private ClientGameManager _clientGame;

    private bool _singlePlayer = false;
    private bool _serverActive = false;
    private bool _clientActive = false;

    private void Start()
    {
        _serverGame = FindObjectOfType<GameManager>();
        _clientGame = FindObjectOfType<ClientGameManager>();

        _serverActive = _serverGame != null;
        _clientActive = _clientGame != null;
    }


    public void ConstructVehicle(int playerId, int vehicleId, byte[] config)
    {
        Vehicle v = Loader.instance.vehicles[vehicleId];
        GameObject obj = Instantiate(v.prefab);
        
//        obj.G
        

    }
    
}
