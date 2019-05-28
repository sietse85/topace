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
            config[0] = (byte) 1;
            config[1] = (byte) 1;
            // since spawn menu isnt done yet, we spawn shuttles for testing (1)
            RequestSpawn packet = new RequestSpawn(_game.playerId, 1, config);
            _gameClient.Send(packet);
        }

        public void RemoveVehicle(NetDataReader r)
        {
            RemoveVehicle rv = new RemoveVehicle();
            rv.Deserialize(r);

            if (_game._vehicles.ContainsKey(rv.playerId))
            {
                NetworkTransform[] networkTransforms =
                    _game._vehicles[rv.playerId].GetComponentsInChildren<NetworkTransform>();
                foreach (NetworkTransform t in networkTransforms)
                {
                    if (_game.networkTransforms.ContainsKey(t.networkTransformId))
                    {
                        _game.networkTransforms.Remove(t.networkTransformId);
                    }
                }
                Destroy(_game._vehicles[rv.playerId]);
                _game._vehicles.Remove(rv.playerId);
            }

            Debug.Log("Removed vehicle from player " + rv.playerId);
        }
    }
}