using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Objects/Weapon")]
    public class Weapon : ScriptableObject
    {
        public int itemId;
        public GameObject prefab;
        public Projectile projectile;
        public float cooldownSec;
        public float energyDrain;

    }
}