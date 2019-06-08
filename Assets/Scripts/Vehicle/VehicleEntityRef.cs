using Client;
using Server;
using UnityEngine;

namespace Vehicle
{
    public class VehicleEntityRef : MonoBehaviour
    {
        public byte playerId;
        public bool isServer;

        public void IsServer(bool itIs)
        {
            isServer = itIs;
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