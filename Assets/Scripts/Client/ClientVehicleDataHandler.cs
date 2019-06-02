using System;
using LiteNetLib.Utils;
using Network;
using Server;
using UnityEngine;

namespace Client
{
    public class ClientVehicleDataHandler : MonoBehaviour
    {
        private GameClient _gameClient;
        private ClientGameManager _game;
        private ByteHelper b;
        private byte[] floatBuf;

        private void Start()
        {
            floatBuf = new byte[4];
            b = gameObject.AddComponent<ByteHelper>();
            _gameClient = GetComponent<GameClient>();
            _game = GetComponent<ClientGameManager>();
        }

        public void TestSpawn()
        {
            Debug.Log("test spawn");
            byte[] config = new byte[8];
            config[0] = 1;
            config[1] = 1;
            // since spawn menu isnt done yet, we spawn shuttles for testing (1)
            RequestSpawn packet = new RequestSpawn(_game.playerId, _game.securityPin, 1, config);
            _gameClient.Send(packet);
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
                float currentArmor = b.ByteToFloat(floatBuf);
                Buffer.BlockCopy(bytes, index, floatBuf, 0, sizeof(float));
                index += sizeof(float); 
                float currentHealth = b.ByteToFloat(floatBuf);
                Buffer.BlockCopy(bytes, index, floatBuf, 0, sizeof(float));
                index += sizeof(float);
                float currentShield = b.ByteToFloat(floatBuf);
                Buffer.BlockCopy(bytes, index, floatBuf, 0, sizeof(float));
                float currentBattery = b.ByteToFloat(floatBuf);
                index += sizeof(float);

                _game.vehicleEntities[playerId].currentArmor = currentArmor;
                _game.vehicleEntities[playerId].currentHealth = currentHealth;
                _game.vehicleEntities[playerId].currentShield = currentShield;
                _game.vehicleEntities[playerId].battery = currentBattery;
            }

        }

        public void RemoveVehicle(NetDataReader r)
        {
            RemoveVehicle rv = new RemoveVehicle();
            rv.Deserialize(r);

            NetworkTransform[] networkTransforms =
                _game.vehicleEntities[rv.playerId].obj.GetComponentsInChildren<NetworkTransform>();
            
            foreach (NetworkTransform t in networkTransforms)
            {
                _game.networkTransforms[t.networkTransformId].slotOccupied = false;
                _game.networkTransforms[t.networkTransformId].processInTick = false;
            }
            Destroy(_game.vehicleEntities[rv.playerId].obj);
            _game.vehicleEntities[rv.playerId].processInTick = false;
    

            Debug.Log("Removed vehicle from player " + rv.playerId);
        }
    }
}