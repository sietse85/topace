using Client;
using UnityEngine;

namespace VehicleFunctions
{
    public class FoiledWings : MonoBehaviour
    {
        public float idleAngle;
        public float unfoilAngle;
        private Vector3 v;
        private ClientGameManager _client;

        private bool foiled = false;

        public void Start()
        {
            _client = FindObjectOfType<ClientGameManager>();

            if (FindObjectOfType<Server.Server>() != null)
                enabled = false;

            transform.Rotate(0f, 0f, 0f);
        }

        public void FoilWings()
        {
            if (foiled)
            {
                v.z = idleAngle;
                transform.localEulerAngles = v;
                foiled = false;
            }
            else
            {
                v.z = unfoilAngle;
                transform.localEulerAngles = v;
                foiled = true;
            }
        }
    }
}