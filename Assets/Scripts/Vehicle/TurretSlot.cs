using System.Collections;
using Network;
using Resource;
using Scriptable;
using UnityEngine;
using Client;
using Server;
using UI;

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
        public byte controlledByPlayerId;
        public Camera cam;

        private void Awake()
        {
            firePacket = new FireWeapon();
            firePacket.HeaderByte = HeaderBytes.FireWeapon;
        }

        public void InitTurret(Weapon w)
        {
            Instantiate(w.prefab, transform);
            cooldown = w.cooldownSec;
            weaponDataBaseId = w.itemId;
            projectileDatabaseId = Loader.instance.weapons[w.itemId].projectile.itemId;

            if (ClientGameManager.instance is ClientGameManager)
            {
                firePacket.PlayerPin = ClientGameManager.instance.securityPin;
                firePacket.PlayerId = ClientGameManager.instance.playerId;
                firePacket.ProjectileDatabaseId = projectileDatabaseId;
                isServer = false;
            }
            else
            {
                isServer = true;
            }
        }

        public void Fire(Vector3 rotation)
        {
            //able to fire
            if (!inCoolDown)
            {

                inCoolDown = true;

                GameObject obj = Instantiate(
                    Loader.instance.weapons[weaponDataBaseId].projectile.prefab,
                    transform.position,
                    transform.rotation
                );
                

                if (!isServer)
                {
                    cam = CameraManager.instance.cam;
                    Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                    Vector3 point = ray.GetPoint(Loader.instance.projectiles[projectileDatabaseId].timeToLive);
                    obj.transform.LookAt(point);

                    firePacket.RotX = point.x;
                    firePacket.RotY = point.y;
                    firePacket.RotZ = point.z;
                }
                else
                {
                    obj.transform.LookAt(rotation);
                }
                
                ProjectileEntity p = obj.GetComponent<ProjectileEntity>();
                p.timeToLive = Loader.instance.weapons[weaponDataBaseId].projectile.timeToLive;
                p.velocity = Loader.instance.weapons[weaponDataBaseId].projectile.projectileSpeed;
                p.projectileDataBaseId = projectileDatabaseId;
                p.shotByPlayer = firePacket.PlayerId;

                if (isServer)
                {
                    GameManager.instance.projectiles[firePacket.PlayerId * 100 + firePacket.UniqueProjectileId].obj = obj;
                    GameManager.instance.projectiles[firePacket.PlayerId * 100 + firePacket.UniqueProjectileId].active =
                        true;
                    GameManager.instance.projectiles[firePacket.PlayerId * 100 + firePacket.UniqueProjectileId].entity = p;
                }
                else
                {
                    ClientGameManager.instance.projectiles[firePacket.PlayerId * 100 + firePacket.UniqueProjectileId].obj = obj;
                    ClientGameManager.instance.projectiles[firePacket.PlayerId * 100 + firePacket.UniqueProjectileId]
                        .active = true;
                }

                
                
                if (!isServer)
                {
                    p.doRayCast = true;
                }
                
                //send the shot to the server
                if (!isServer)
                {
                    ClientGameManager.instance.uniqueProjectileId++;
                    if (ClientGameManager.instance.uniqueProjectileId == 100)
                    {
                        ClientGameManager.instance.uniqueProjectileId = 0;
                    }

                    firePacket.UniqueProjectileId = ClientGameManager.instance.uniqueProjectileId;
                    firePacket.WeaponSlotFired = turretSlotNumber;
                    GameClient.instance.Send(firePacket);
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
