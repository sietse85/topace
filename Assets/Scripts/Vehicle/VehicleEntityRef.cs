using Client;
using Server;
using UnityEngine;

namespace VehicleFunctions
{
    public class VehicleEntityRef : MonoBehaviour
    {
        public byte playerId;
        public GameManager server;
        public ClientGameManager client;
        public bool isServer;

        private void Awake()
        {
            server = FindObjectOfType<GameManager>();
            client = FindObjectOfType<ClientGameManager>();
            if (server is GameManager)
            {
                isServer = true;
            }
            else
            {
                isServer = false;
            }
        }

        public VehicleEntity GetReference()
        {
            if (isServer)
            {
                return server.vehicleEntities[playerId];
            }
            
            return client.vehicleEntities[playerId];
        }
    }
}