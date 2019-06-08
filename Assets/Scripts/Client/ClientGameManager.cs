using System.Collections.Generic;
using LiteNetLib;
using Menu;
using UnityEngine;
using Network;
using Server;
using Vehicle;

namespace Client
{
    public class ClientGameManager : MonoBehaviour
    {
        public static ClientGameManager instance;
        
        public NetworkTransformStruct[] networkTransforms;
        public VehicleEntity[] vehicleEntities;
        public Player[] players;
        public ProjectileReference[] projectiles; 
        private Dictionary<int, string> _playerNames;
        public Transform multiplayerMenu;
        public Transform spawnMenu;
        public Transform mainMenu;
        public VehicleConstructor vc;
        public byte playerId;
        public ClientPlayerDataHandler playerDataHandler;
        public ClientVehicleDataHandler vehicleDataHandler;
        public NetworkTransformHandler networkTransformHandler;
        public VehicleController vehicleController;
        public ProjectileHandler projectileHandler;
        public int securityPin;
        public byte uniqueProjectileId;
        public byte ticknumber;
        public int index;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            networkTransforms = new NetworkTransformStruct[1024];
            vehicleEntities = new VehicleEntity[254];
            projectiles = new ProjectileReference[254 * 100];
            playerDataHandler = gameObject.GetComponent<ClientPlayerDataHandler>();
            vehicleDataHandler = gameObject.GetComponent<ClientVehicleDataHandler>();
            networkTransformHandler = gameObject.GetComponent<NetworkTransformHandler>();
            vehicleController = gameObject.GetComponent<VehicleController>();
            projectileHandler = gameObject.GetComponent<ProjectileHandler>();
            vc = gameObject.GetComponent<VehicleConstructor>();
            players = new Player[64];
        }

        public void HandleReceived(NetPacketReader r)
        {
            byte header = r.GetByte();

            if (header == HeaderBytes.IncomingSnapShot)
            {
                index = 0;
                byte[] snapshot = r.GetRemainingBytes();
                ticknumber = snapshot[index];
                index++;
                
                while (index < snapshot.Length)
                {
                    byte commandHeader = snapshot[index];
                    index++;
                    switch (commandHeader)
                    {
                        case HeaderBytes.SendPlayerData:
                            playerDataHandler.UpdatePlayerData(ref snapshot);
                            break;
                        case HeaderBytes.SendVehicleData:
                            vehicleDataHandler.UpdateVehicleInfo(ref snapshot);
                            break;
                        case HeaderBytes.FireWeapon:
                            projectileHandler.SpawnProjectile(ref snapshot);
                            break;
                        case HeaderBytes.NetworkTransFormId:
                            networkTransformHandler.UpdateNetworkTransform(ref snapshot);
                            break;
                    }

                }
            }
            else
            {
                switch (header)
                {
                    case HeaderBytes.AskClientForUsername:
                        playerDataHandler.SendPlayerName("Unknown");
                        break;
                    case HeaderBytes.SendPlayerId:
                        playerDataHandler.SetPlayerId(r);
                        vehicleDataHandler.TestSpawn();
                        break;
                    case HeaderBytes.OpenSpawnMenuOnClient:
                        break;
                    case HeaderBytes.SpawnVehicle:
                        vc.ConstructVehicleFromPacket(r);
                        break;

                    case HeaderBytes.NetworkTransFormsForVehicle:
                        networkTransformHandler.SetTransformIds(r);
                        break;
                    case HeaderBytes.RemoveVehicle:
                        vehicleDataHandler.RemoveVehicle(r);
                        break;
                }
            }
        }

        
    }
}
