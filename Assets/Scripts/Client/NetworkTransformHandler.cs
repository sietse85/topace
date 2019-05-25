using LiteNetLib;
using Network;
using UnityEngine;

namespace Client
{
    public class NetworkTransformHandler : MonoBehaviour
    {
        // Start is called before the first frame update
        private GameClient _gameClient;
        private ClientGameManager _game;
        private NetworkTransformUpdate nt;
        private Vector3 loc;
        private Quaternion rot;
        private NetworkTransformsForVehicle vnt;

        private void Start()
        {
            _gameClient = GetComponent<GameClient>();
            _game = GetComponent<ClientGameManager>();
            nt = new NetworkTransformUpdate();
            loc = new Vector3();
            rot = new Quaternion();
            vnt = new NetworkTransformsForVehicle();
            vnt.HeaderByte = HeaderBytes.NetworkTransFormsForVehicle;
        }

        public void UpdateNetworkTransform(NetPacketReader r)
        {
            nt.Deserialize(r);

            if (_game.networkTransforms.ContainsKey(nt.NetworkTransformId))
            {
                loc.x = nt.LocX;
                loc.y = nt.LocY;
                loc.z = nt.LocZ;
                rot.x = nt.RotX;
                rot.y = nt.RotY;
                rot.z = nt.RotZ;
                rot.w = nt.RotW;

                //only update networktransform of different players
                if (nt.PlayerId != _game.playerId)
                {
                    _game.networkTransforms[nt.NetworkTransformId].SetPositionAndRotation(loc, rot);
                }
            }
        }

        public void SetTransformIds(NetPacketReader r)
        {
            Debug.Log("set transform ids");
            vnt.Deserialize(r);

            NetworkTransform[] transforms = _game._vehicles[vnt.VehicleId].GetComponentsInChildren<NetworkTransform>();

            int i = 0;

            foreach (NetworkTransform t in transforms)
            {
                t.SetPlayerId(vnt.PlayerId);
                t.SetTransformId(vnt.NetworkTransformIds[i]);
                _game.networkTransforms.Add(vnt.NetworkTransformIds[i], t.networkTransform);
                i++;
                if (_game.playerId == vnt.PlayerId)
                {
                    t.InitUpdates();
                }
            }
        }
    }
}
