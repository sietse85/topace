using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Objects/Vehicle")]
    public class VehicleScriptable : ScriptableObject
    {
        public int itemId;
        public string name;
        public GameObject prefab;
        public float baseHealth;
        public float baseShield;
        public float baseArmor;
        public float rollSpeed;
        public float yawSpeed;
        public float pitchSpeed;
        public bool isLandVehicle;
        public float acceleration;
        public float maximumSpeed;
        public float batteryCapacity;
        public float rechargePerSec;
        public float shieldRechargePerSec;
        public int moduleSlots;
        public int weaponSlots;
    }
}