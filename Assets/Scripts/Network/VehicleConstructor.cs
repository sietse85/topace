using Client;
using LiteNetLib;
using Resource;
using Scriptable;
using UnityEngine;
using Server;
using Vehicle;

namespace Network
{
    public class VehicleConstructor : MonoBehaviour
    {
        private NetworkTransformsForVehicle vnt;
        private Ticker ticker;
        private bool isServer;

        private void Start()
        {

            isServer = false;

            if (GameManager.instance is GameManager)
            {
                if (ClientGameManager.instance == null)
                    isServer = true;
            }
            
            Debug.Log("Did we initializa as a server " + isServer);

            vnt = new NetworkTransformsForVehicle();
            vnt.HeaderByte = HeaderBytes.NetworkTransFormsForVehicle;
            ticker = FindObjectOfType<Ticker>();
        }

        public void SpawnExistingVehiclesOnClient(NetPeer peer, int playerId)
        {
            foreach (VehicleEntity v in GameManager.instance.vehicleEntities)
            {
                if (v.playerId == playerId)
                    continue;
                
                if(!v.processInTick)
                    continue;
                
                Vector3 pos = GameManager.instance.vehicleEntities[v.playerId].obj.transform.position;
                Quaternion rot = GameManager.instance.vehicleEntities[v.playerId].obj.transform.rotation; 
                ConstructVehicleSpawnPacket(v.playerId, v.vehicleDatabaseId, pos, rot, v.config, peer);
                NetworkTransform[] childs = GameManager.instance.vehicleEntities[v.playerId].obj.GetComponentsInChildren<NetworkTransform>();
                int[] ntIds = new int[childs.Length];
                int i = 0;
                foreach (NetworkTransform t in childs)
                {
                    ntIds[i] = t.networkTransformId;
                    i++;
                }
                vnt.PlayerId = v.playerId;
                vnt.NetworkTransformIds = ntIds;
                GameServer.instance.Send(vnt, peer);
            }
        }

        public void ConstructVehicleFromPacket(NetPacketReader r)
        {
            SpawnVehicle n = new SpawnVehicle();
            n.Deserialize(r);

            Vector3 pos = new Vector3(n.PosX, n.PosY, n.PosZ);
            Quaternion rot = new Quaternion(n.RotX, n.RotY, n.RotZ, n.RotW);

            ConstructVehicle(n.PlayerId, n.VehicleDatabaseId, pos, rot, n.Config);
        }

        public void ConstructVehicle(byte playerId, int vehicleDatabaseId, Vector3 pos, Quaternion rot,
            byte[] config)
        {
            VehicleScriptable v = Loader.instance.vehicles[vehicleDatabaseId];
            GameObject obj = Instantiate(v.prefab, pos, rot);

            if (isServer)
                obj.GetComponent<Rigidbody>().isKinematic = true;
            
            Debug.Log("Setting the reference to " + playerId);
            
            

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

            if (isServer)
            {
                GameManager.instance.turrets[playerId] = turretSlots;
                int[] ntIds = AssignNewNetworkTransforms(ref childs, playerId, pos, rot);
                ConstructVehicleSpawnPacket(playerId, vehicleDatabaseId, pos, rot, config, null);
                AddToEntityList(playerId, v, vehicleDatabaseId, config);

                vnt.PlayerId = playerId;
                vnt.NetworkTransformIds = ntIds;
                Debug.Log(ntIds.Length);
                foreach (int i in ntIds)
                {
                    Debug.Log("Sending id " + i);
                }
                GameServer.instance.SendToAll(vnt);
            }

            if (!isServer)
            {
                if (ClientGameManager.instance.playerId != playerId)
                {
                    obj.GetComponent<Rigidbody>().isKinematic = true;
                }

                //if the vehicle is spawned for this player
                if (playerId == ClientGameManager.instance.playerId)
                {
                    ClientGameManager.instance.vehicleController.GiveControlToPlayer(obj, vehicleDatabaseId);
                }
            }
            
            VehicleEntityRef r = obj.AddComponent<VehicleEntityRef>();
            r.playerId = playerId;

            //finaly set vehicle refs
            if (isServer)
            {
                GameManager.instance.vehicleEntities[playerId].obj = obj;
                GameManager.instance.vehicleEntities[playerId].playerId = playerId;
            }
            else
            {
                ClientGameManager.instance.vehicleEntities[playerId].obj = obj;
                ClientGameManager.instance.vehicleEntities[playerId].playerId = playerId;
                ClientGameManager.instance.vehicleEntities[playerId].processInTick = true;
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
                GameServer.instance.SendToAll(s);
            }
            else
            {
                GameServer.instance.Send(s, peer);
            }
        }

        public void AddToEntityList(byte playerId, VehicleScriptable v, int vehicleDatabaseId, byte[] config)
        {
            GameManager.instance.vehicleEntities[playerId].playerId = playerId;
            GameManager.instance.vehicleEntities[playerId].vehicleDatabaseId = vehicleDatabaseId;
            GameManager.instance.vehicleEntities[playerId].currentHealth = v.baseHealth;
            GameManager.instance.vehicleEntities[playerId].currentArmor = v.baseArmor;
            GameManager.instance.vehicleEntities[playerId].currentShield = v.baseShield;
            GameManager.instance.vehicleEntities[playerId].shieldRechargeRate = v.shieldRechargePerSec;
            GameManager.instance.vehicleEntities[playerId].battery = v.batteryCapacity;
            GameManager.instance.vehicleEntities[playerId].batteryRechargeRate = v.rechargePerSec;
            GameManager.instance.vehicleEntities[playerId].moduleSlots = v.moduleSlots;
            GameManager.instance.vehicleEntities[playerId].weaponSlots = v.weaponSlots;
            GameManager.instance.vehicleEntities[playerId].ApplyConfigurationOfVehicle(config);
            GameManager.instance.vehicleEntities[playerId].processInTick = true;
        }

        public void CreateWeaponsOnVehicle(GameObject obj, byte[] config, VehicleScriptable v) 
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

        public int[] AssignNewNetworkTransforms(ref NetworkTransform[] childs, byte playerId, Vector3 position, Quaternion rotation)
        {
            int[] ntIds = new int[childs.Length];
            int i = 0;

            foreach (NetworkTransform t in childs)
            {
                int newNetworkTransformId = GetNextFreeNetworkTransformSlot();
                t.SetTransformId(newNetworkTransformId);
                t.SetPlayerId(playerId);
                GameManager.instance.ticker.networkTransforms[newNetworkTransformId].playerId = playerId;
                GameManager.instance.ticker.networkTransforms[newNetworkTransformId].processInTick = true;
                GameManager.instance.ticker.networkTransforms[newNetworkTransformId].transform = t.transform;
                GameManager.instance.ticker.networkTransforms[newNetworkTransformId].transform.position = position;
                GameManager.instance.ticker.networkTransforms[newNetworkTransformId].transform.rotation = rotation;
                ntIds[i] = newNetworkTransformId;
                i++;
            }

            return ntIds;
        }
    }
    
}
