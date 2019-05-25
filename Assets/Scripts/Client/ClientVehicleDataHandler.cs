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
            byte[] config = new byte[6];
            config[0] = (byte) 1;
            config[1] = (byte) 1;
            RequestSpawn packet = new RequestSpawn(_game.playerId, 1, config);
            _gameClient.Send(packet);
        }
    }
}