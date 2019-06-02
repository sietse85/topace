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
        private Ticker ticker;

        private void Start()
        {
            _serverGame = FindObjectOfType<GameManager>();
            _clientGame = FindObjectOfType<ClientGameManager>();

            vnt = new NetworkTransformsForVehicle();
            vnt.HeaderByte = HeaderBytes.NetworkTransFormsForVehicle;
            ticker = FindObjectOfType<Ticker>();
        }

        public void SpawnExistingVehiclesOnClient(NetPeer peer, int playerId)
        {
            foreach (VehicleEntity v in _serverGame.VehicleEntities)
            {
                if (v.playerId == playerId)
                    continue;
                
                if(!v.processInTick)
                    continue;
                
                Vector3 pos = _serverGame.VehicleEntities[v.playerId].obj.transform.position;
                Quaternion rot = _serverGame.VehicleEntities[v.playerId].obj.transform.rotation; 
                ConstructVehicleSpawnPacket(v.playerId, v.vehicleDatabaseId, pos, rot, v.config, peer);
                NetworkTransform[] childs = _serverGame.VehicleEntities[v.playerId].obj.GetComponentsInChildren<NetworkTransform>();
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

        public void ConstructVehicle(byte playerId, int vehicleDatabaseId, Vector3 pos, Quaternion rot,
            byte[] config, bool isServer = false)
        {
            Vehicle v = Loader.instance.vehicles[vehicleDatabaseId];
            GameObject obj = Instantiate(v.prefab, pos, rot);

            NetworkTransform[] childs = obj.GetComponentsInChildren<NetworkTransform>();
            TurretSlot[] turretSlots = obj.GetComponentsInChildren<TurretSlot>();
            byte currentSlot = 0;
            foreach (TurretSlot slot in turretSlots)
            {
                slot.turretSlotNumber = currentSlot;
                slot.controllerByPlayerId = playerId;
                currentSlot++;
            }
            
            CreateWeaponsOnVehicle(obj, config, v);

            if (_serverGame != null && !isServer)
            {
                _serverGame.turrets[playerId] = turretSlots;
                int[] ntIds = AssignNewNetworkTransforms(ref childs, playerId, pos, rot);
                ConstructVehicleSpawnPacket(playerId, vehicleDatabaseId, pos, rot, config, null);
                _serverGame.VehicleEntities[playerId].obj = obj;
                AddVehicleToServerVehicleEntitiesList(playerId, v, vehicleDatabaseId, config);

                vnt.PlayerId = playerId;
                vnt.NetworkTransformIds = ntIds;
                foreach (int i in ntIds)
                {
                    Debug.Log("Sending id " + ntIds[i]);
                }
                _serverGame.gameServer.SendToAll(vnt);
            }

            if (_clientGame != null)
            {
                _clientGame.VehicleEntities[playerId].obj = obj;

                //if the vehicle is spawned for this player
                if (playerId == _clientGame.playerId)
                {
                    _clientGame.vehicleController.GiveControlToPlayer(obj, vehicleDatabaseId);
                }
            }
        }

        public void ConstructVehicleSpawnPacket(byte playerId, int vehicleDatabaseId, Vector3 pos,
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

        public void AddVehicleToServerVehicleEntitiesList(byte playerId, Vehicle v, int vehicleDatabaseId, byte[] config)
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

        public int GetNextFreeNetworkTransformSlot()
        {
            for (int i = 0; i < ticker.networkTransforms.Length; i++)
            {
                if (!ticker.networkTransforms[i].slotOccupied)
                {
                    ticker.networkTransforms[i].slotOccupied = true;
                    Debug.Log("Next free slot is " + i);
                    return i;
                }
            }

            return -1;
        }

        public int[] AssignNewNetworkTransforms(ref NetworkTransform[] childs, int playerId, Vector3 position, Quaternion rotation)
        {
            int[] ntIds = new int[childs.Length];
            int i = 0;

            foreach (NetworkTransform t in childs)
            {
                int newNetworkTransformId = GetNextFreeNetworkTransformSlot();
                t.SetTransformId(newNetworkTransformId);
                t.SetPlayerId(playerId);
                _serverGame.ticker.networkTransforms[newNetworkTransformId].position = position;
                _serverGame.ticker.networkTransforms[newNetworkTransformId].rotation = rotation;
                _serverGame.ticker.networkTransforms[newNetworkTransformId].playerId = playerId;
                _serverGame.ticker.networkTransforms[newNetworkTransformId].processInTick = true;
                _serverGame.ticker.networkTransforms[newNetworkTransformId].transform = t.transform;
                ntIds[i] = newNetworkTransformId;
                i++;
            }

            return ntIds;
        }
    }
    
}
