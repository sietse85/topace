using System.Collections.Generic;
using UnityEngine;
using Scriptable;

namespace Resource
{
    public class Loader : MonoBehaviour
    {
        public static Loader instance;

        public Dictionary<int, Weapon> weapons;
        public Dictionary<int, Vehicle> vehicles;
        public Dictionary<int, Module> modules;
        public Dictionary<int, Projectile> projectiles;

        private void Awake()
        {
            if (null == instance)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
                return;
            }

            weapons = new Dictionary<int, Weapon>();
            vehicles = new Dictionary<int, Vehicle>();
            modules = new Dictionary<int, Module>();
            projectiles = new Dictionary<int, Projectile>();
            Weapon[] loadWeapons = Resources.LoadAll<Weapon>("");
            Vehicle[] loadVehicles = Resources.LoadAll<Vehicle>("");
            Debug.Log(loadVehicles.Length);
            Module[] loadModules = Resources.LoadAll<Module>("");
            Projectile[] loadProjectiles = Resources.LoadAll<Projectile>("");

            foreach (Weapon w in loadWeapons)
            {
                weapons.Add(w.itemId, w);
            }

            foreach (Vehicle v in loadVehicles)
            {
                vehicles.Add(v.itemId, v);
            }

            foreach (Projectile p in loadProjectiles)
            {
                projectiles.Add(p.itemId, p);
            }

            foreach (Module m in loadModules)
            {
                modules.Add(m.itemId, m);
            }
        }
    }
}
