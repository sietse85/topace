using System;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
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

    private NetworkTransformsForVehicle vnt;

    private void Start()
    {
        _serverGame = FindObjectOfType<GameManager>();
        _clientGame = FindObjectOfType<ClientGameManager>();

        _serverActive = _serverGame != null;
        _clientActive = _clientGame != null;
        vnt = new NetworkTransformsForVehicle();
        vnt.headerByte = HeaderBytes.NetworkTransFormsForVehicle;
    }

    public void ConstructVehicleFromPacket(NetPacketReader r)
    {
        SpawnShip n = new SpawnShip();
        n.Deserialize(r);
        
        Vector3 pos = new Vector3(n.posx, n.posy, n.posz);
        Quaternion rot = new Quaternion(n.rotx, n.roty, n.rotz, n.rotw);
        
        ConstructVehicle(n.playerId, n.vehicleId, pos, rot, n.config, true);
    }


    public void ConstructVehicle(int playerId, int vehicleId, Vector3 pos, Quaternion rot, byte[] config, bool skipServer = false)
    {
        Vehicle v = Loader.instance.vehicles[vehicleId];
        GameObject obj = Instantiate(v.prefab, pos, rot);

        if (_serverGame != null && !skipServer)
        {
            NetworkTransform[] childs = obj.GetComponentsInChildren<NetworkTransform>();

            int[] ntIds = new int[childs.Length];
            int i = 0;
            
            foreach (NetworkTransform t in childs)
            {
                _serverGame.networkTransformId++;
                t.SetTransformId(_serverGame.networkTransformId);
                t.SetPlayerId(playerId);
                _serverGame.networkTransforms.Add(_serverGame.networkTransformId, t);
                ntIds[i] = _serverGame.networkTransformId;
                i++;
            }

            if (!_serverGame.vehicles.ContainsKey(vehicleId))
                _serverGame.vehicles.Add(vehicleId, obj);
            
            ConstructVehicleSpawnPacket(playerId, vehicleId, pos, rot, config);

            vnt.playerId = playerId;
            vnt.vehicleId = vehicleId;
            vnt.networkTransformIds = ntIds;
            
            _serverGame.server.SendToAll(vnt);
        }

        if (_clientGame != null)
        {
            if (!_clientGame._vehicles.ContainsKey(vehicleId))
                _clientGame._vehicles.Add(vehicleId, obj);
        }
    }

    public void ConstructVehicleSpawnPacket(int playerId, int vehicleId, Vector3 pos, Quaternion rot, byte[] config)
    {
        SpawnShip s = new SpawnShip(playerId, vehicleId, pos.x, pos.y, pos.z, rot.x, rot.y, rot.z, rot.w, config );
        s.headerByte = HeaderBytes.SpawnShipOnClient;
        
        _serverGame.server.SendToAll(s);
    }
}
