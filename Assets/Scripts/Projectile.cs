using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 3f;
    public float projectileLifetime = 5f;

    public int damage = 1;
    private Rigidbody2D rb;

    private Player player = null;

    // Start is called before the first frame update
    Quaternion isoBase;

    public bool isReverseBullet = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        StartCoroutine(DestroyAfterTime());
    }

    public void SetStats(int pDamage, float projectileSpeed = 3f, float lifetime = 5f)
    {
        damage = pDamage;
        speed = projectileSpeed;
        projectileLifetime = lifetime;
    }

    void LateUpdate()
    {
        float z = transform.eulerAngles.z;
        Quaternion faceZ = Quaternion.AngleAxis(z, Vector3.forward);

        // Apply Z spin on top of the iso tilt (order matters!)
        transform.GetChild(0).rotation = isoBase * faceZ;
        // If it spins the “wrong” way, try the other order:
        // childSprite.rotation = faceZ * isoBase;
    }

    //ontriggerenter2D
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }

        if (isReverseBullet)
        {
            IReversible reversible = other.GetComponent<IReversible>();
            if (reversible != null)
            {
                
                reversible.Reverse(1);
                Destroy(gameObject);
            }
            return;
        }
        
        // check if other object has the IDamageable interface:
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && !other.CompareTag("Player")) //TODO: nuke this later, no enemies have projectiles
        {
            if (player != null)
            {
                player.HitTarget();
            }
            damageable.TakeDamage(damage);
            Debug.Log("Hit " + other.name + " for " + damage);
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(projectileLifetime);
        if (player != null)
        {
            player.MissTarget();
        }
        Destroy(gameObject);
    }
}
