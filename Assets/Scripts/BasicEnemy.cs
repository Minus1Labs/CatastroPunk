using Unity.Mathematics;
using UnityEngine;

public class BasicEnemy : Enemy
{

    [Header("Basic Enemy Properties")]
    [SerializeField] private float damageInterval = 1f; // seconds between ticks
    [SerializeField] private float detectionRadius = 6f;

    private float damageTimer = 0f;
    private Vector2 moveDirection;
    private Transform player;

    protected override void Awake()
    {
        base.Awake();

        // Decide initial direction
        moveDirection = new Vector2(1f, 0f).normalized;

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
        }
        else
        {
            rb.velocity = Vector2.zero; // stop when player is out of range
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
        var damageable = target.GetComponent<IDamageable>();
        if (damageable != null && Time.time >= damageTimer)
        {
            damageable.TakeDamage(baseDamage);
            damageTimer = Time.time + damageInterval;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Attack(other.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    protected override void Die()
    {
        throw new System.NotImplementedException();
    }

    public override void Reverse(int strength)
    {
        throw new System.NotImplementedException();
    }
}