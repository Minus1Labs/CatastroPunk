using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IReversible
{
    public float speed = 3f;
    public float projectileLifetime = 5f;

    public float damage = 1;
    private Rigidbody2D rb;

    private Player player = null;

    // Start is called before the first frame update
    Quaternion isoBase;

    public bool isReverseBullet = false;

    public bool damagesPlayer = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        StartCoroutine(DestroyAfterTime());
    }

    public void SetStats(float pDamage, float projectileSpeed = 3f, float lifetime = 5f)
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
            IReversible reversible = other.GetComponentInParent<IReversible>();
            Debug.Log("Found Reversible: " + reversible);
            //if not found, then try top most parent:



            if (reversible != null)
            {

                reversible.Reverse(1);
                Destroy(gameObject);
            }
            return;
        }

        // check if other object has the IDamageable interface:
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && !other.CompareTag("Player") && !damagesPlayer) //TODO: nuke this later, no enemies have projectiles
        {
            if (player != null)
            {
                //player.HitTarget();
            }
            damageable.TakeDamage(damage);
            Destroy(gameObject);
        }

        if (damageable != null && damagesPlayer && other.CompareTag("Player"))
        {
            damageable.TakeDamage(1);
        }


    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(projectileLifetime);
        if (player != null)
        {
            //player.MissTarget();
        }
        Destroy(gameObject);
    }

    // implementing IReversible
    public void Reverse(int direction)
    {
        if (!damagesPlayer) return; // only reverse if it can damage the player
        speed = -speed;
        rb.velocity = transform.right * speed;
        damagesPlayer = false;
        SetStats(80, this.speed, this.projectileLifetime); // make it do more damage to enemies
        // // flip the sprite
        // Vector3 localScale = transform.localScale;
        // localScale.y *= -1;
        // transform.localScale = localScale;
    }
}
