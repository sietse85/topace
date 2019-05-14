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

        public void SpawnShip(string prefab)
        {
            RequestSpawn packet = new RequestSpawn(_game.playerId, Prefabs.GetFromString("shuttle"));
            _client.Send(packet);
        }
    }
}