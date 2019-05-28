using System.Collections;
using System.Collections.Generic;
using Scriptable;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class TurretSlot : MonoBehaviour
{
    public float cooldown;
    public float currentCooldown;
    public GameObject prefab;


    public void InitTurret(Weapon w)
    {
        prefab = Instantiate(w.prefab, transform);
        cooldown = w.cooldownSec;
    }

    public void Fire()
    {
        if (currentCooldown == 0f)
        {
            currentCooldown = cooldown;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentCooldown > 0f)
        {
            currentCooldown -= cooldown / 100f;
        }
    }
}
