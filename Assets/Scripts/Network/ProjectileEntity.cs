using UnityEngine;
using VehicleFunctions;

public class ProjectileEntity : MonoBehaviour
{

    public float timeToLive;
    public float velocity;
    public bool doRayCast;
    private RaycastHit hit;
    public int projectileId;
   
    // Update is called once per frame
    void Update()
    {
        timeToLive -= 100f * Time.deltaTime;

        if (timeToLive < 0f)
        {
            Destroy(gameObject);
        }

        if (doRayCast)
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                if (hit.collider.gameObject.layer == 8)
                {
                    GameObject obj = hit.collider.gameObject;
                    Debug.Log(obj);
                    VehicleEntityRef v = obj.GetComponentInParent<VehicleEntityRef>();
                    byte playerId = v.GetReference().playerId;
                    Debug.Log("A projectile collied with player " + playerId);
                }
            }
        }
    
        transform.position += transform.forward * velocity * Time.deltaTime;
    }
}
