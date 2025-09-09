using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 3f;
    public float projectileLifetime = 5f;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    Quaternion isoBase;

    public bool isReverseBullet = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        StartCoroutine(DestroyAfterTime());
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
            damageable.TakeDamage(1);
            Destroy(gameObject);
        }
    }
    
    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(projectileLifetime);
        Destroy(gameObject);
    }
}
