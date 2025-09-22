using UnityEngine;

public class AntBossProjectile : Enemies.GenericEnemyProjectile
{
    //These are required, but I can't require 2 of the same type with [RequireComponent]

    [SerializeField]
    private GameObject PuddlePrefab;

    [SerializeField]
    private GameObject ToxicExplosionPrefab;

    public override void OnProjectileRemove()
    {
        GameObject puddleInstance = Instantiate(PuddlePrefab, transform.position, transform.rotation);
        GameObject toxicExplosionPrefab = Instantiate(ToxicExplosionPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}