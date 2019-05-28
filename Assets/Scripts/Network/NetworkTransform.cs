using System.Collections;
using Client;
using UnityEngine;

namespace Network
{
    public class NetworkTransform : MonoBehaviour
    {
        public Transform networkTransform;
        [SerializeField] private int ownedByPlayerId;
        [SerializeField] public int networkTransformId;
        private GameClient _client;
        public NetworkTransformUpdate u;
        private ClientGameManager _game;

        private void Awake()
        {
            ownedByPlayerId = -1;
            networkTransformId = -1;
            _client = FindObjectOfType<GameClient>();
            u = new NetworkTransformUpdate();
            u.HeaderByte = HeaderBytes.NetworkTransFormId;
            _game = FindObjectOfType<ClientGameManager>();
            networkTransform = gameObject.transform;
        }

        // Start is called before the first frame update
        public void InitUpdates()
        {
            if (_client != null)
            {
                if (ownedByPlayerId != _game.playerId)
                    enabled = false;
                StartCoroutine(SendUpdateToServer());
            }
            else
            {
                enabled = false;
            }
        }

        IEnumerator SendUpdateToServer()
        {
            while (true)
            {
                if (networkTransformId == -1)
                    yield break;
                u.LocX = networkTransform.position.x;
                u.LocY = networkTransform.position.y;
                u.LocZ = networkTransform.position.z;
                u.RotX = networkTransform.rotation.x;
                u.RotY = networkTransform.rotation.y;
                u.RotZ = networkTransform.rotation.z;
                u.RotW = networkTransform.rotation.w;
                u.NetworkTransformId = networkTransformId;
                u.PlayerId = _game.playerId;
                _client.Send(u);
                yield return new WaitForSeconds(_client.updateSpeedNetworktransforms);
            }
        }

        public int GetTransformId()
        {
            return networkTransformId;
        }

        public int GetPlayerId()
        {
            return ownedByPlayerId;
        }

        public void SetTransformId(int id)
        {
            networkTransformId = id;
        }

        public void SetPlayerId(int id)
        {
            ownedByPlayerId = id;
        }
    }
}
