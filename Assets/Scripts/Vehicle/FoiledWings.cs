using Client;
using UnityEngine;

namespace Vehicle
{
    public class FoiledWings : MonoBehaviour
    {
        public float idleAngle;
        public float unfoilAngle;
        private Vector3 v;
        private bool foiled;

        public void Start()
        {
            if (FindObjectOfType<Server.GameServer>() != null)
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