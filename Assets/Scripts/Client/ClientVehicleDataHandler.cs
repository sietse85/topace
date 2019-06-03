using System;
using LiteNetLib.Utils;
using Network;
using Server;
using UnityEngine;

namespace Client
{
    public class ClientVehicleDataHandler : MonoBehaviour
    {
        private byte[] floatBuf;

        private void Start()
        {
            floatBuf = new byte[4];
        }

        public void TestSpawn()
        {
            Debug.Log("test spawn");
            byte[] config = new byte[8];
            config[0] = 1;
            config[1] = 1;
            // since spawn menu isnt done yet, we spawn shuttles for testing (1)
            RequestSpawn packet = new RequestSpawn(ClientGameManager.instance.playerId, ClientGameManager.instance.securityPin, 1, config);
            GameClient.instance.Send(packet);
        }

        public void UpdateVehicleInfo(NetDataReader r)
        {
            byte[] bytes = r.GetRemainingBytes();
            int index = 0;
            while (index < bytes.Length)
            {
                byte playerId = bytes[index];
                index += sizeof(byte);
                Buffer.BlockCopy(bytes, index, floatBuf, 0, sizeof(float));
                index += sizeof(float); 
                float currentArmor = ByteHelper.instance.ByteToFloat(floatBuf);
                Buffer.BlockCopy(bytes, index, floatBuf, 0, sizeof(float));
                index += sizeof(float); 
                float currentHealth = ByteHelper.instance.ByteToFloat(floatBuf);
                Buffer.BlockCopy(bytes, index, floatBuf, 0, sizeof(float));
                index += sizeof(float);
                float currentShield = ByteHelper.instance.ByteToFloat(floatBuf);
                Buffer.BlockCopy(bytes, index, floatBuf, 0, sizeof(float));
                float currentBattery = ByteHelper.instance.ByteToFloat(floatBuf);
                index += sizeof(float);

                ClientGameManager.instance.vehicleEntities[playerId].currentArmor = currentArmor;
                ClientGameManager.instance.vehicleEntities[playerId].currentHealth = currentHealth;
                ClientGameManager.instance.vehicleEntities[playerId].currentShield = currentShield;
                ClientGameManager.instance.vehicleEntities[playerId].battery = currentBattery;
            }

        }

        public void RemoveVehicle(NetDataReader r)
        {
            RemoveVehicle rv = new RemoveVehicle();
            rv.Deserialize(r);

            NetworkTransform[] networkTransforms =
                ClientGameManager.instance.vehicleEntities[rv.playerId].obj.GetComponentsInChildren<NetworkTransform>();
            
            foreach (NetworkTransform t in networkTransforms)
            {
                ClientGameManager.instance.networkTransforms[t.networkTransformId].slotOccupied = false;
                ClientGameManager.instance.networkTransforms[t.networkTransformId].processInTick = false;
            }
            Destroy(ClientGameManager.instance.vehicleEntities[rv.playerId].obj);
            ClientGameManager.instance.vehicleEntities[rv.playerId].processInTick = false;
    

            Debug.Log("Removed vehicle from player " + rv.playerId);
        }
    }
}