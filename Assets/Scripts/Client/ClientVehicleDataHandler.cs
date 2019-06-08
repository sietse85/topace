using System;
using LiteNetLib.Utils;
using Network;
using Server;
using UnityEngine;

namespace Client
{
    public class ClientVehicleDataHandler : MonoBehaviour
    {
        private byte[] _floatBuf;

        private void Start()
        {
            _floatBuf = new byte[4];
        }

        public void TestSpawn()
        {
            byte[] config = new byte[8];
            config[0] = 1;
            config[1] = 1;
            // since spawn menu isnt done yet, we spawn shuttles for testing (1)
            RequestSpawn packet = new RequestSpawn(ClientGameManager.instance.playerId, ClientGameManager.instance.securityPin, 1, config);
            GameClient.instance.Send(packet);
        }

        public void UpdateVehicleInfo(ref byte[] snapshot)
        {
            int index = ClientGameManager.instance.index;
            byte playerId = snapshot[index];
            index += sizeof(byte);
            Buffer.BlockCopy(snapshot, index, _floatBuf, 0, sizeof(float));
            index += sizeof(float); 
            float currentArmor = ByteHelper.instance.ByteToFloat(_floatBuf);
            Buffer.BlockCopy(snapshot, index, _floatBuf, 0, sizeof(float));
            index += sizeof(float); 
            float currentHealth = ByteHelper.instance.ByteToFloat(_floatBuf);
            Buffer.BlockCopy(snapshot, index, _floatBuf, 0, sizeof(float));
            index += sizeof(float);
            float currentShield = ByteHelper.instance.ByteToFloat(_floatBuf);
            Buffer.BlockCopy(snapshot, index, _floatBuf, 0, sizeof(float));
            float currentBattery = ByteHelper.instance.ByteToFloat(_floatBuf);
            index += sizeof(float);

            ClientGameManager.instance.vehicleEntities[playerId].currentArmor = currentArmor;
            ClientGameManager.instance.vehicleEntities[playerId].currentHealth = currentHealth;
            ClientGameManager.instance.vehicleEntities[playerId].currentShield = currentShield;
            ClientGameManager.instance.vehicleEntities[playerId].battery = currentBattery;
            ClientGameManager.instance.index = index;
        }

        public void RemoveVehicle(NetDataReader r)
        {
            RemoveVehicle rv = new RemoveVehicle();
            rv.Deserialize(r);

            NetworkTransform[] networkTransforms =
                ClientGameManager.instance.vehicleEntities[rv.PlayerId].obj.GetComponentsInChildren<NetworkTransform>();
            
            foreach (NetworkTransform t in networkTransforms)
            {
                ClientGameManager.instance.networkTransforms[t.networkTransformId].slotOccupied = false;
                ClientGameManager.instance.networkTransforms[t.networkTransformId].processInTick = false;
            }
            Destroy(ClientGameManager.instance.vehicleEntities[rv.PlayerId].obj);
            ClientGameManager.instance.vehicleEntities[rv.PlayerId].processInTick = false;
    

        }
    }
}