using UnityEngine;

namespace VehicleFunctions
{
    public struct VehicleEntity
    {
        public byte playerId;
        public int vehicleDatabaseId;
        public bool processInTick;
        public float currentHealth;
        public float maxHealth;
        public float currentShield;
        public float maxShield;
        public float currentArmor;
        public float maxArmor;
        public float battery;
        public float batteryRechargeRate;
        public float shieldRechargeRate;
        public int moduleSlots;
        public int weaponSlots;
        public float maxYaw;
        public float maxRoll;
        public float maxPitch;
        public float maxSpeed;
        public GameObject obj;
        
        // in case the vehicle has manable turrets
        public bool isOccupiedByOtherPlayer;
        public byte[] config;

        public void ApplyConfigurationOfVehicle(byte[] config)
        {
            this.config = config;
            maxHealth = currentHealth;
            maxShield = currentShield;
            maxArmor = currentArmor;
        }
    }
}