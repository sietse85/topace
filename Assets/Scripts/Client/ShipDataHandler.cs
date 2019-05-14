using UnityEngine;

namespace Network
{
    public class ClientShipDataHandler : MonoBehaviour
    {
        private Client _client;
        private ClientGameManager _game;

        private void Start()
        {
            _client = GetComponent<Client>();
            _game = GetComponent<ClientGameManager>();
        }

        public void TestSpawn()
        {
            byte[] config = new byte[6];
            config[0] = (byte) 1;
            config[1] = (byte) 1;
            RequestSpawn packet = new RequestSpawn(_game.playerId, 1, config);
        }
    }
}