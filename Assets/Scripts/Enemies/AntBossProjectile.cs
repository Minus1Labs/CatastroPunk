//using UnityEditor.Callbacks;
using UnityEngine;

public class AntBossProjectile : Enemies.GenericEnemyProjectile, IReversible
{
    //These are required, but I can't require 2 of the same type with [RequireComponent]

    [SerializeField]
    private GameObject PuddlePrefab;

    [SerializeField]
    private GameObject ToxicExplosionPrefab;

    public bool damageEnemy;

    public override void OnProjectileRemove()
    {
        GameObject puddleInstance = Instantiate(PuddlePrefab, transform.position, transform.rotation);
        GameObject toxicExplosionPrefab = Instantiate(ToxicExplosionPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    public void Reverse(int direction)
    {
        rb.velocity = -rb.velocity;
        projectileTeam = 1; // now damages enemies
        // damage = 100;
    }

    
}