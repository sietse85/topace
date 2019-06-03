using System.Collections;
using Network;
using Resource;
using Scriptable;
using UnityEngine;
using Client;
using Server;

namespace Vehicle
{
    public class TurretSlot : MonoBehaviour
    {
        public float cooldown;
        public bool inCoolDown;
        public int weaponDataBaseId;
        public int projectileDatabaseId;
        public byte turretSlotNumber;
        public FireWeapon firePacket;
        private bool isServer;
        private float latencyCompensation;
        public int controllerByPlayerId;

        private void Awake()
        {
            firePacket = new FireWeapon();
            firePacket.headerByte = HeaderBytes.FireWeapon;
        }

        public void InitTurret(Weapon w)
        {
            Instantiate(w.prefab, transform);
            cooldown = w.cooldownSec;
            weaponDataBaseId = w.itemId;
            projectileDatabaseId = Loader.instance.weapons[w.itemId].projectile.itemId;

            if (ClientGameManager.instance is ClientGameManager)
            {
                firePacket.playerPin = ClientGameManager.instance.securityPin;
                firePacket.playerId = ClientGameManager.instance.playerId;
                firePacket.projectileDatabaseId = projectileDatabaseId;
                isServer = false;
            }
            else
            {
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
                    ClientGameManager.instance.uniqueProjectileId++;
                    if (ClientGameManager.instance.uniqueProjectileId == 100)
                    {
                        ClientGameManager.instance.uniqueProjectileId = 0;
                    }

                    firePacket.uniqueProjectileId = ClientGameManager.instance.uniqueProjectileId;
                    firePacket.weaponSlotFired = turretSlotNumber;
                    GameClient.instance.Send(firePacket);
                }

                inCoolDown = true;
                GameObject obj = Instantiate(
                    Loader.instance.weapons[weaponDataBaseId].projectile.prefab,
                    transform.position,
                    transform.rotation
                );

                if (isServer)
                {
                    GameManager.instance.projectiles[firePacket.playerId * 100 + firePacket.uniqueProjectileId].obj = obj;
                }
                else
                {
                    ClientGameManager.instance.projectiles[firePacket.playerId * 100 + firePacket.uniqueProjectileId].obj = obj;
                }

                ProjectileEntity p = obj.GetComponent<ProjectileEntity>();
                p.timeToLive = Loader.instance.weapons[weaponDataBaseId].projectile.timeToLive;
                p.velocity = Loader.instance.weapons[weaponDataBaseId].projectile.projectileSpeed;
                p.projectileDataBaseId = projectileDatabaseId;
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
}
