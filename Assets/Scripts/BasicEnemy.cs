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

    [SerializeField] private float reverseLookbackSeconds = 2f;
    [SerializeField] private float historyKeepSeconds = 2f;

    private struct PosSample { public float t; public Vector2 p; public PosSample(float t, Vector2 p){ this.t=t; this.p=p; } }
    private readonly System.Collections.Generic.LinkedList<PosSample> posHistory = new System.Collections.Generic.LinkedList<PosSample>();

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

        posHistory.Clear();
        posHistory.AddLast(new PosSample(Time.time, transform.position));
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
        posHistory.AddLast(new PosSample(Time.time, transform.position));
        float cutoff = Time.time - Mathf.Max(historyKeepSeconds, reverseLookbackSeconds + 0.25f);
        while (posHistory.Count > 1 && posHistory.First.Value.t < cutoff)
            posHistory.RemoveFirst();
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
        // lookback = 1s by default; multiply by strength if you want (keep 1s: ignore strength)
        float lookback = reverseLookbackSeconds; // or: reverseLookbackSeconds * Mathf.Max(1, strength);
        float targetT = Time.time - Mathf.Max(0.01f, lookback);

        if (posHistory.Count == 0) return;

        // Find the first sample with time <= targetT (scan from newest backward for fewer steps)
        var node = posHistory.Last;
        while (node != null && node.Value.t > targetT) node = node.Previous;

        Vector2 targetPos;
        if (node == null)
        {
            // All samples newer than target -> use oldest we have
            targetPos = posHistory.First.Value.p;
        }
        else
        {
            // Interpolate with the next newer sample if it exists
            var newer = node.Next;
            if (newer == null || Mathf.Approximately(node.Value.t, newer.Value.t))
            {
                targetPos = node.Value.p;
            }
            else
            {
                float u = Mathf.InverseLerp(node.Value.t, newer.Value.t, targetT);
                targetPos = Vector2.Lerp(node.Value.p, newer.Value.p, u);
            }
        }

        // Teleport + stop motion
        rb.position = targetPos;
        rb.velocity = Vector2.zero;

        // (Optional tiny cleanup) write a fresh sample now so history stays consistent
        posHistory.AddLast(new PosSample(Time.time, rb.position));
    }


    // private IEnumerator FreezeForSeconds(float seconds)
    // {
    //     float originalSpeed = baseSpeed;
    //     baseSpeed = 0;
    //     rb.velocity = Vector2.zero;
    //     yield return new WaitForSeconds(seconds);
    //     baseSpeed = originalSpeed;
    // }
}