using System;
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
        public NetworkTransformUpdate u;

        private void Awake()
        {
            ownedByPlayerId = -1;
            networkTransformId = -1;
            u = new NetworkTransformUpdate();
            u.HeaderByte = HeaderBytes.NetworkTransFormId;
            networkTransform = gameObject.transform;
        }

        public void InitUpdates()
        {
            if (GameClient.instance != null)
            {
                if (ownedByPlayerId != ClientGameManager.instance.playerId)
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
                u.PlayerId = ClientGameManager.instance.playerId;
                u.PlayerPin = ClientGameManager.instance.securityPin;
                GameClient.instance.Send(u);
                yield return new WaitForSeconds(GameClient.instance.updateSpeedNetworktransforms);
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
