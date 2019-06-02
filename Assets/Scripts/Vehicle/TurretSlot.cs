using System.Collections;
using Network;
using Resource;
using Scriptable;
using UnityEngine;
using Client;
using Server;

public class TurretSlot : MonoBehaviour
{
    public float cooldown;
    public bool inCoolDown;
    public int weaponDataBaseId;
    public int projectileId;
    public byte turretSlotNumber;
    public FireWeapon firePacket;
    public ClientGameManager client;
    private GameManager game;
    private bool isServer;
    private float latencyCompensation;
    public int controllerByPlayerId;

    private void Awake()
    {
        firePacket = new FireWeapon();
        firePacket.headerByte = HeaderBytes.FireWeapon;
        client = FindObjectOfType<ClientGameManager>();
    }
    
    public void InitTurret(Weapon w)
    {
        Instantiate(w.prefab, transform);
        cooldown = w.cooldownSec;
        weaponDataBaseId = w.itemId;
        projectileId = Loader.instance.weapons[w.itemId].projectile.itemId;

        if (client is ClientGameManager)
        {
            firePacket.playerPin = client.securityPin;
            firePacket.playerId = client.playerId;
            firePacket.projectileId = projectileId;
            isServer = false;
        }
        else
        {
            game = FindObjectOfType<GameManager>();
            isServer = true;
        }
    }

    public void Fire()
    {
        //able to fire
        if (!inCoolDown)
        {
            
            if (!isServer)
            {
                client.bulletId++;
                if (client.bulletId == 100)
                {
                    client.bulletId = 0;
                }
                firePacket.bulletId = client.bulletId;
                firePacket.weaponSlotFired = turretSlotNumber;
                client.client.Send(firePacket);
            }

            inCoolDown = true;
            GameObject obj = Instantiate(
                Loader.instance.weapons[weaponDataBaseId].projectile.prefab,
                transform.position,
                transform.rotation
            );

            if (isServer)
            {
                game.projectiles[firePacket.playerId * 100 + firePacket.bulletId].obj = obj;
            }
            else
            {
                client.projectiles[firePacket.playerId * 100 + firePacket.bulletId].obj = obj;
            }

            ProjectileEntity p = obj.GetComponent<ProjectileEntity>();
            p.timeToLive = Loader.instance.weapons[weaponDataBaseId].projectile.timeToLive;
            p.velocity = Loader.instance.weapons[weaponDataBaseId].projectile.projectileSpeed;
            p.projectileId = projectileId;
            if (!isServer)
            {
                p.doRayCast = true;

            }
            
            StartCoroutine(InitiateCoolDown());
        }
    }

    public IEnumerator InitiateCoolDown()
    {
        if (isServer)
        {
            inCoolDown = false;
            yield break;
        }

        yield return new WaitForSeconds(cooldown);
        inCoolDown = false;
    }
}
