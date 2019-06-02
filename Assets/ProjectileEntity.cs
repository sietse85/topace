using UnityEngine;

public class ProjectileEntity : MonoBehaviour
{

    public float timeToLive;
    public float velocity;
    public int unqiueId;
    public byte playerId;
    public int projectileId;
   
    // Update is called once per frame
    void Update()
    {
        timeToLive -= 100f * Time.deltaTime;

        if (timeToLive < 0f)
        {
            Destroy(gameObject);
        }

        transform.position += transform.forward * velocity * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Projectile collided with something");
    }
}
