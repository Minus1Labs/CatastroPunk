using Unity.Mathematics;
using UnityEngine;
using System.Collections;

public class BasicEnemy : Enemy
{

    [Header("Basic Enemy Properties")]
    [SerializeField] private float damageInterval = 1f; // seconds between ticks
    [SerializeField] private float detectionRadius = 6f;

    private float damageTimer = 0f;
    private Transform player;

    public GameObject explosionPrefab;

    public Animator robotAnimator;

    protected override void Awake()
    {
        base.Awake();

        // Find player by tag
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
        }

        Debug.Log("Woke up enemy");
    }

    private void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= detectionRadius)
        {
            Move();
            robotAnimator.SetBool("Follow", true);
        }
        else
        {
            rb.velocity = Vector2.zero; // stop when player is out of range
            robotAnimator.SetBool("Follow", false);
        }
    }

    //simple 4-d movement towards player
    public override void Move()
    {
        if (player == null)
        {
            return;
        }
        Vector2 playerDir = player.position - transform.position;
        playerDir.Normalize();


        if (math.abs(playerDir.x) >= math.abs(playerDir.y))
        {
            playerDir.y = 0;
        }
        else
        {
            playerDir.x = 0;
        }

        rb.velocity = playerDir * baseSpeed;
    }

    public override void Attack(GameObject target)
    {
        Debug.Log("Attacking Player!");
        robotAnimator.SetBool("Attack", true);
        var damageable = target.GetComponent<IDamageable>();
        if (target.transform.parent != null)
        { 
            damageable = target.transform.parent.GetComponent<IDamageable>();
        }
        if (damageable != null && Time.time >= damageTimer)
        {
            damageable.TakeDamage(baseDamage);
            damageTimer = Time.time + damageInterval;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("Colliding with: " + other.name);
        if (other.CompareTag("Player"))
        {
            Attack(other.gameObject);

        }
        else
        {
            robotAnimator.SetBool("Attack", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    protected override void Die()
    {
        if (player != null)
        {
            Player pl = player.GetComponent<Player>();
            if (pl != null)
            {
                pl.KilledEnemy();
            }
        }
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        //TODO: add any other death logic before destroying
        Destroy(gameObject);
    }

    public override void Reverse(int strength)
    {
        // for now, instead of reversing, just freeze for a bit:
        StartCoroutine(FreezeForSeconds(2f * strength));
    }

    private IEnumerator FreezeForSeconds(float seconds)
    {
        float originalSpeed = baseSpeed;
        baseSpeed = 0;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(seconds);
        baseSpeed = originalSpeed;
    }
}