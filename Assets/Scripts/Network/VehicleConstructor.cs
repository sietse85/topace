using Client;
using LiteNetLib;
using Resource;
using UnityEngine;
using Server;
using Scriptable;
using VehicleFunctions;

namespace Network
{
    public class VehicleConstructor : MonoBehaviour
    {
        private GameManager _serverGame;
        private ClientGameManager _clientGame;
        private NetworkTransformsForVehicle vnt;

        private void Start()
        {
            _serverGame = FindObjectOfType<GameManager>();
            _clientGame = FindObjectOfType<ClientGameManager>();

            vnt = new NetworkTransformsForVehicle();
            vnt.HeaderByte = HeaderBytes.NetworkTransFormsForVehicle;
        }

        public void SpawnExistingVehiclesOnClient(NetPeer peer, int playerId)
        {
            foreach (VehicleEntity v in _serverGame.VehicleEntities)
            {
                if (v.playerId == playerId)
                    continue;
                
                if(!v.processInTick)
                    continue;
                
                Vector3 pos = _serverGame.vehicles[v.playerId].transform.position;
                Quaternion rot = _serverGame.vehicles[v.playerId].transform.rotation; 
                ConstructVehicleSpawnPacket(v.playerId, v.vehicleDatabaseId, pos, rot, v.config, peer);
                NetworkTransform[] childs = _serverGame.vehicles[v.playerId].GetComponentsInChildren<NetworkTransform>();
                int[] ntIds = new int[childs.Length];
                int i = 0;
                foreach (NetworkTransform t in childs)
                {
                    ntIds[i] = t.networkTransformId;
                    i++;
                }
                vnt.PlayerId = v.playerId;
                vnt.NetworkTransformIds = ntIds;
                _serverGame.gameServer.Send(vnt, peer);
            }
        }

        public void ConstructVehicleFromPacket(NetPacketReader r)
        {
            SpawnVehicle n = new SpawnVehicle();
            n.Deserialize(r);

            Vector3 pos = new Vector3(n.PosX, n.PosY, n.PosZ);
            Quaternion rot = new Quaternion(n.RotX, n.RotY, n.RotZ, n.RotW);

            ConstructVehicle(n.PlayerId, n.VehicleDatabaseId, pos, rot, n.Config, true);
        }

        public void ConstructVehicle(int playerId, int vehicleDatabaseId, Vector3 pos, Quaternion rot,
            byte[] config, bool skipServer = false)
        {
            Vehicle v = Loader.instance.vehicles[vehicleDatabaseId];
            GameObject obj = Instantiate(v.prefab, pos, rot);

            CreateWeaponsOnVehicle(obj, config, v);

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
                    _serverGame.ticker.networkTransforms.Add(_serverGame.networkTransformId, t);
                    ntIds[i] = _serverGame.networkTransformId;
                    i++;
                }

                ConstructVehicleSpawnPacket(playerId, vehicleDatabaseId, pos, rot, config, null);
                if (_serverGame.vehicles.ContainsKey(playerId))
                    _serverGame.vehicles.Remove(playerId);
                _serverGame.vehicles.Add(playerId, obj);
                AddVehicleToServerVehicleEntitiesList(playerId, v, vehicleDatabaseId, config);

                vnt.PlayerId = playerId;
                vnt.NetworkTransformIds = ntIds;
                _serverGame.gameServer.SendToAll(vnt);
            }

            if (_clientGame != null)
            {
                if (!_clientGame._vehicles.ContainsKey(playerId))
                    _clientGame._vehicles.Remove(playerId);
                
                _clientGame._vehicles.Add(playerId, obj);

                //if the vehicle is spawned for this player
                if (playerId == _clientGame.playerId)
                {
                    _clientGame.vehicleController.GiveControlToPlayer(obj, vehicleDatabaseId);
                }
            }
        }

        public void ConstructVehicleSpawnPacket(int playerId, int vehicleDatabaseId, Vector3 pos,
            Quaternion rot, byte[] config, NetPeer peer)
        {
            Debug.Log("Spawning existing vehicle from the gameworld on this client..." + playerId);
            SpawnVehicle s = new SpawnVehicle(playerId, vehicleDatabaseId, pos.x, pos.y, pos.z, rot.x, rot.y,
                rot.z, rot.w, config);
            s.HeaderByte = HeaderBytes.SpawnVehicle;

            if (peer == null)
            {
                _serverGame.gameServer.SendToAll(s);
            }
            else
            {
                _serverGame.gameServer.Send(s, peer);
            }
        }

        public void AddVehicleToServerVehicleEntitiesList(int playerId, Vehicle v, int vehicleDatabaseId, byte[] config)
        {
            _serverGame.VehicleEntities[playerId].playerId = playerId;
            _serverGame.VehicleEntities[playerId].vehicleDatabaseId = vehicleDatabaseId;
            _serverGame.VehicleEntities[playerId].currentHealth = v.baseHealth;
            _serverGame.VehicleEntities[playerId].currentArmor = v.baseArmor;
            _serverGame.VehicleEntities[playerId].currentShield = v.baseShield;
            _serverGame.VehicleEntities[playerId].shieldRechargeRate = v.shieldRechargePerSec;
            _serverGame.VehicleEntities[playerId].battery = v.batteryCapacity;
            _serverGame.VehicleEntities[playerId].batteryRechargeRate = v.rechargePerSec;
            _serverGame.VehicleEntities[playerId].moduleSlots = v.moduleSlots;
            _serverGame.VehicleEntities[playerId].weaponSlots = v.weaponSlots;
            _serverGame.VehicleEntities[playerId].ApplyConfigurationOfVehicle(config);
            _serverGame.VehicleEntities[playerId].processInTick = true;
        }

        public void CreateWeaponsOnVehicle(GameObject obj, byte[] config, Vehicle v) 
        {
            TurretSlot[] slots = obj.GetComponentsInChildren<TurretSlot>();
            for (int i = 0; i < v.weaponSlots; i++)
            {
                slots[i].InitTurret(Loader.instance.weapons[config[i]]);
            }
        }
    }
    
}
