using Client;
using Server;
using UnityEngine;

namespace Vehicle
{
    public class VehicleEntityRef : MonoBehaviour
    {
        public byte playerId;
        public bool isServer;

        private void Awake()
        {
            if (GameManager.instance is GameManager)
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
                return GameManager.instance.vehicleEntities[playerId];
            }
            
            return ClientGameManager.instance.vehicleEntities[playerId];
        }
    }
}