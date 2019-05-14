using UnityEngine;

namespace Network
{
    public class ClientPlayerDataHandler : MonoBehaviour
    {
        private Client _client;
        private ClientGameManager _game;

        private void Start()
        {
            _client = GetComponent<Client>();
            _game = GetComponent<ClientGameManager>();
        }
         
        public void SendPlayerName()
        {
            SendUserName packet = new SendUserName("Sietse");
            _client.Send(packet); 
        }

        public void SetPlayerId(int playerId)
        {
            Debug.Log("Client playerId = " + playerId);
            _game.playerId = playerId;
            _game.shipDataHandler.SpawnShip("shuttle");
        }
    }
}