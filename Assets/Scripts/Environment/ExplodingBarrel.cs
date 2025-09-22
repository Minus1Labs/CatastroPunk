using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBarrel : MonoBehaviour, IDamageable
{
    public GameObject explosionEffect;

    

    public void TakeDamage(int damage)
    {
        // instantaite explosion effect and destroy self
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    

}
