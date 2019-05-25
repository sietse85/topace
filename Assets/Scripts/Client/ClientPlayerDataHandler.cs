using Network;
using UnityEngine;

namespace Client
{
    public class ClientPlayerDataHandler : MonoBehaviour
    {
        private GameClient _gameClient;
        private ClientGameManager _game;

        private void Start()
        {
            _gameClient = GetComponent<GameClient>();
            _game = GetComponent<ClientGameManager>();
        }
         
        public void SendPlayerName()
        {
            SendUserNameToServer packet = new SendUserNameToServer("Sietse");
            _gameClient.Send(packet); 
        }

        public void SetPlayerId(int playerId)
        {
            Debug.Log("Client playerId = " + playerId);
            _game.playerId = playerId;
        }
    }
}