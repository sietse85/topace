using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using UnityEngine;
using Network;

namespace Server
{
    public class GameManager : MonoBehaviour
    {
        public Dictionary<int, NetworkTransform> networkTransforms;
        public Dictionary<int, NetPeer> players;
        public Dictionary<int, string> playerNames;
        public Dictionary<int, GameObject> vehicles;
        public Server server;
        public PlayerDataHandler playerDataHandler;
        public VehicleDataHandler vehicleDataHandler;
        public VehicleConstructor vc;
        public int vehicleId;
        public int playerId;
        public int networkTransformId;
        public float updateSpeedNetworktransforms = 0.2f;
        private NetworkTransformUpdate u;

        // Start is called before the first frame update
        void Start()
        {
            networkTransforms = new Dictionary<int, NetworkTransform>();
            vehicles = new Dictionary<int, GameObject>();
            players = new Dictionary<int, NetPeer>();
            playerNames = new Dictionary<int, string>();
            server = GetComponent<Server>();
            playerDataHandler = gameObject.GetComponent<PlayerDataHandler>();
            vehicleDataHandler = gameObject.GetComponent<VehicleDataHandler>();
            vc = gameObject.AddComponent<VehicleConstructor>();
            u = new NetworkTransformUpdate();
            StartCoroutine(SendActualNetworkTransformPositionsToClients());
        }

        // Update is called once per frame
        public void HandleReceived(NetPeer peer, NetPacketReader r)
        {
            byte header = r.GetByte();

            switch (header)
            {
                case HeaderBytes.SendUserNameToServer:
                    playerDataHandler.ReceivePlayerName(peer, r);
                    break;
                case HeaderBytes.RequestSpawn:
                    vehicleDataHandler.PlayerRequestedShipSpawn(peer, r);
                    break;
                case HeaderBytes.NetworkTransFormId:
                    vehicleDataHandler.UpdateVehicleTransform(r);
                    break;
            }
        }

        public IEnumerator SendActualNetworkTransformPositionsToClients()
        {
            while (true)
            {
                foreach (KeyValuePair<int, NetworkTransform> t in networkTransforms)
                {
                    if (t.Value.networkTransform == null)
                        continue;

                    u.LocX = t.Value.networkTransform.position.x;
                    u.LocY = t.Value.networkTransform.position.y;
                    u.LocZ = t.Value.networkTransform.position.z;
                    u.RotX = t.Value.networkTransform.rotation.x;
                    u.RotY = t.Value.networkTransform.rotation.y;
                    u.RotZ = t.Value.networkTransform.rotation.z;
                    u.NetworkTransformId = t.Value.networkTransformId;
                    u.PlayerId = t.Value.GetPlayerId();

                    server.SendToAll(u);
                }

                yield return new WaitForSeconds(updateSpeedNetworktransforms);
            }
        }
    }
}
