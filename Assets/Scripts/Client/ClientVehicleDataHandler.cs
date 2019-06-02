using LiteNetLib.Utils;
using Network;
using UnityEngine;

namespace Client
{
    public class ClientVehicleDataHandler : MonoBehaviour
    {
        private GameClient _gameClient;
        private ClientGameManager _game;

        private void Start()
        {
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

        public void RemoveVehicle(NetDataReader r)
        {
            RemoveVehicle rv = new RemoveVehicle();
            rv.Deserialize(r);

            NetworkTransform[] networkTransforms =
                _game.VehicleEntities[rv.playerId].obj.GetComponentsInChildren<NetworkTransform>();
            
            foreach (NetworkTransform t in networkTransforms)
            {
                _game.networkTransforms[t.networkTransformId].slotOccupied = false;
                _game.networkTransforms[t.networkTransformId].processInTick = false;
            }
            Destroy(_game.VehicleEntities[rv.playerId].obj);
            _game.VehicleEntities[rv.playerId].processInTick = false;
    

            Debug.Log("Removed vehicle from player " + rv.playerId);
        }
    }
}