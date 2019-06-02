using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Objects/Projectile")]
    public class Projectile : ScriptableObject
    {
        public int itemId;
        public GameObject prefab;
        public float projectileSpeed;
        public float damage;
        public float shieldPenetration;
        public float armorPenetration;
        public bool applyDrop;
        public float startDropAfterDisctance;
        public float dropSpeed;
        public float timeToLive;
    }
}