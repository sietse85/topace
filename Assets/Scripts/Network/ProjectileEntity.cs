using System;
using Client;
using Network;
using Resource;
using Server;
using UnityEngine;
using Vehicle;

public class ProjectileEntity : MonoBehaviour
{

    public float timeToLive;
    public float velocity;
    public bool doRayCast;
    public int projectileDataBaseId;
    public byte uniqueProjectileId;
    public byte shotByPlayer;
    private LayerMask mask;
    private bool isServer;

    private void Start()
    {
        isServer = ClientGameManager.instance == null;
    }

    // Update is called once per frame
    void Update()
    {
        timeToLive -= 100f * Time.deltaTime;

        if (timeToLive < 0f)
        {
            if (isServer)
            {
                GameManager.instance.projectiles[shotByPlayer * 100 + uniqueProjectileId].active = false;
            }
            else
            {
                ClientGameManager.instance.projectiles[shotByPlayer * 100 + uniqueProjectileId].active = false;
            }
            
            Destroy(gameObject);
        }

        if (doRayCast)
        {
            DoRayCast(true);
        }

        mask = ~0;

        transform.position += transform.forward * velocity * Time.deltaTime;
    }

    public byte? DoRayCast(bool sendReport = false)
    {
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            Loader.instance.projectiles[projectileDataBaseId].sizeDiameter,
            mask
        );
        
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                GameObject obj = colliders[i].gameObject;
                VehicleEntityRef v = obj.GetComponentInParent<VehicleEntityRef>();
                
                //client only;
                if (sendReport)
                {
                    SendDamageReport(v.GetReference().playerId, transform.position);
                    break;
                }
                
                return v.GetReference().playerId;
            }
        }

        return null;
    }

    public void SendDamageReport(byte playerId, Vector3 impactPos)
    {
        ReportCollision rep = new ReportCollision
        {
            HeaderByte = HeaderBytes.ReportCollision,
            PlayerIdThatWasHit = playerId,
            PlayerPin = ClientGameManager.instance.securityPin,
            UniqueProjectileId = uniqueProjectileId,
            ByProjectileDatabaseId = projectileDataBaseId,
            TickWhenCollisionOccurred = ClientGameManager.instance.ticknumber,
            impactPosX =  impactPos.x,
            impactPosY = impactPos.y,
            impactPosZ = impactPos.z
        };
        GameClient.instance.Send(rep);
    }
}
