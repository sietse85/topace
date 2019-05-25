using Client;
using LiteNetLib;
using Resource;
using UnityEngine;
using Server;
using Scriptable;

namespace Network
{
    public class VehicleConstructor : MonoBehaviour
    {
        private GameManager _serverGame;
        private ClientGameManager _clientGame;

        private bool _singlePlayer = false;

        private NetworkTransformsForVehicle vnt;

        private void Start()
        {
            _serverGame = FindObjectOfType<GameManager>();
            _clientGame = FindObjectOfType<ClientGameManager>();

            vnt = new NetworkTransformsForVehicle();
            vnt.HeaderByte = HeaderBytes.NetworkTransFormsForVehicle;
        }

        public void ConstructVehicleFromPacket(NetPacketReader r)
        {
            SpawnVehicle n = new SpawnVehicle();
            n.Deserialize(r);

            Vector3 pos = new Vector3(n.PosX, n.PosY, n.PosZ);
            Quaternion rot = new Quaternion(n.RotX, n.RotY, n.RotZ, n.RotW);

            ConstructVehicle(n.PlayerId, n.VehicleDatabaseId, n.VehicleId, pos, rot, n.Config, true);
        }

        public void ConstructVehicle(int playerId, int vehicleDatabaseId, int vehicleId, Vector3 pos, Quaternion rot,
            byte[] config, bool skipServer = false)
        {
            Vehicle v = Loader.instance.vehicles[vehicleDatabaseId];
            GameObject obj = Instantiate(v.prefab, pos, rot);

            if (_serverGame != null && !skipServer)
            {
                NetworkTransform[] childs = obj.GetComponentsInChildren<NetworkTransform>();

                int[] ntIds = new int[childs.Length];
                int i = 0;

                foreach (NetworkTransform t in childs)
                {
                    _serverGame.networkTransformId++;
                    t.SetTransformId(_serverGame.networkTransformId);
                    t.SetPlayerId(playerId);
                    _serverGame.networkTransforms.Add(_serverGame.networkTransformId, t);
                    ntIds[i] = _serverGame.networkTransformId;
                    i++;
                }

                _serverGame.vehicleId++;
                ConstructVehicleSpawnPacket(playerId, vehicleDatabaseId, _serverGame.vehicleId, pos, rot, config);
                _serverGame.vehicles.Add(_serverGame.vehicleId, obj);

                vnt.PlayerId = playerId;
                vnt.VehicleId = _serverGame.vehicleId;
                vnt.NetworkTransformIds = ntIds;
                _serverGame.server.SendToAll(vnt);
            }

            if (_clientGame != null)
            {
                if (!_clientGame._vehicles.ContainsKey(vehicleId))
                    _clientGame._vehicles.Add(vehicleId, obj);

                //if the vehicle is spawned for this player
                if (playerId == _clientGame.playerId)
                {
                    _clientGame.vehicleController.GiveControlToPlayer(obj, vehicleDatabaseId);
                }
            }
        }

        public void ConstructVehicleSpawnPacket(int playerId, int vehicleDatabaseId, int vehicleId, Vector3 pos,
            Quaternion rot, byte[] config)
        {
            SpawnVehicle s = new SpawnVehicle(playerId, vehicleDatabaseId, vehicleId, pos.x, pos.y, pos.z, rot.x, rot.y,
                rot.z, rot.w, config);
            s.HeaderByte = HeaderBytes.SpawnVehicle;

            _serverGame.server.SendToAll(s);
        }
    }
}
