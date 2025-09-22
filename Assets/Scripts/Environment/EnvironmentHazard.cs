
using System.Collections;
using UnityEditor.Timeline.Actions;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnvironmentHazard : MonoBehaviour
{

    [SerializeField, Tooltip("Does damage based on tickrate, does not create i-frames")]
    private bool continuous = false;

    [SerializeField, Tooltip("Damage per tick, or damage per instance. Truncates to int if continuous is false")]
    private float damage = 0f;

    [SerializeField, Tooltip("How often it damages the player")]
    private float ticksPerSecond = 1f;

    [SerializeField, Tooltip("How long this hazard lasts. -1 for inf. This is checked every half second")]
    private float lifetimeSeconds = -1f;
    private float createTime;
    private float lastTickTime = 0.0f;
    private bool isCheckingLifetime = false;


    void Awake()
    {
        createTime = Time.time;
    }

    void Update()
    {
        if (lifetimeSeconds >= 0 && !isCheckingLifetime)
        {
            StartCoroutine(LifetimeRoutine());
        }
    }

    IEnumerator LifetimeRoutine()
    {
        isCheckingLifetime = true;
        yield return new WaitForSeconds(0.5f);
        if (Time.time - createTime >= lifetimeSeconds)
        {
            Destroy(gameObject);
        }
        isCheckingLifetime = false;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player p = collision.GetComponent<Player>();
            if (Time.time - lastTickTime >= ticksPerSecond)
            {
                if (continuous)
                {
                    p.TakeContinuousDamage(damage);
                }
                else
                {
                    p.TakeDamage((int)damage);
                }
                lastTickTime = Time.time;
            }
        }
    }

}