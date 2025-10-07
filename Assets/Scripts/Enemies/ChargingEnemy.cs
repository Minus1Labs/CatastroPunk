using Unity.Mathematics;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;

public class ChargingEnemy : Enemies.Enemy, IEnemyCommunicator
{

    [Header("Basic Enemy Properties")]
    [SerializeField] private float damageInterval = 1f; // seconds between ticks
    [SerializeField] private float detectionRadius = 12f;
    [SerializeField] private bool alertsOtherEnemies = true;

    [SerializeField] private float chargeWindupDuration = 0.5f;
    [SerializeField] private float chargeDuration = 1.2f;
    [SerializeField] private float chargeSpeed = 12f;
    [SerializeField] private float chargeEndlagDuration = 3f;
    [SerializeField] private float directionSwitchLockout = 1.5f;    
    private float damageTimer = 0f;
    private Transform player;

    public GameObject explosionPrefab;
    private Animator antAnimator;

    public enum ChargingEnemyState
    {
        Uninitiated,
        Idle,
        Chasing,
        CrowdControlled,
        Dead
    }

    private ChargingEnemyState enemyState;

    public List<Coroutine> coroutines = new();
    private bool charging = false;
    private bool canCharge = true;
    private bool isIdleMoving = false;
    private bool hasTarget = false;
    private bool inChargeEndlag = false;

    private Vector2 lastMoveDirection = new(0, 0);
    private float lastMoveTime; 
    


    [SerializeField] private float reverseLookbackSeconds = 3f;
    [SerializeField] private float historyKeepSeconds = 2f;

    private struct PosSample { public float t; public Vector2 p; public PosSample(float t, Vector2 p){ this.t=t; this.p=p; } }
    private readonly System.Collections.Generic.LinkedList<PosSample> posHistory = new System.Collections.Generic.LinkedList<PosSample>();

    protected override void Awake()
    {
        base.Awake();
        antAnimator = GetComponent<Animator>();
        // Find player by tag
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
        }

        Debug.Log("Woke up enemy");

        enemyState = ChargingEnemyState.Idle;

        posHistory.Clear();
        posHistory.AddLast(new PosSample(Time.time, transform.position));
    }

    private void Update()
    {
        if (player == null) return;

        if (!hasTarget)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist <= detectionRadius)
            {
                hasTarget = true;
            }
        }


        if (hasTarget)
        {
            if (alertsOtherEnemies)
            {
                //go through list of objects tagged enemy, then call NotifyMeOfPlayer() on every 
                //enemy that implements IEnemyCommunicator
            }
            if (enemyState == ChargingEnemyState.Idle)
            {
                enemyState = ChargingEnemyState.Chasing;
            }
        }
        
        if (enemyState == ChargingEnemyState.Idle || enemyState == ChargingEnemyState.Chasing)
        {
            Move();
        }

        posHistory.AddLast(new PosSample(Time.time, transform.position));
        float cutoff = Time.time - Mathf.Max(historyKeepSeconds, reverseLookbackSeconds + 0.25f);
        while (posHistory.Count > 1 && posHistory.First.Value.t < cutoff)
            posHistory.RemoveFirst();
    }

    //simple 4-d movement towards player
    public override void Move()
    {
        if (enemyState == ChargingEnemyState.Idle)
        {
            IdleMove();
        }
        else if (enemyState == ChargingEnemyState.Chasing)
        {
            ActiveMove();
        } 
    }

    /*

    */
    public void ActiveMove()
    {
        if (charging)
        {
            if (inChargeEndlag) { rb.velocity *= 0.9f; }
            return;
        }
        if (player == null)
        {
            Debug.Log("ChargingEnemy has target, but player is null!");
            return;
        }
        Vector2 playerDir = player.position - transform.position;
        playerDir.Normalize();
        Vector2 playerGridDirection = ScriptUtilities.SnapToGridDirection(playerDir);

        //Magic number for grid alignment because the vectors are never equal in testing?
        if (canCharge && Vector2.Dot(playerGridDirection, playerDir) > .999f)
        {
            Debug.Log("Starting charge!");
            Coroutine chargeRoutine = StartCoroutine(ChargeCoroutine(playerGridDirection));
            coroutines.Add(chargeRoutine);
        }
        else
        {
            if (Time.time - lastMoveTime >= directionSwitchLockout)
            {
                rb.velocity = playerGridDirection.normalized * baseSpeed;
                lastMoveDirection = playerGridDirection.normalized;
                lastMoveTime = Time.time;
            }
            else
            {
                rb.velocity = lastMoveDirection.normalized * baseSpeed;
            }
            
        }
    }

    IEnumerator ChargeCoroutine(Vector2 direction)
    {
        charging = true;
        canCharge = false;
        //play charge windup animation here
        yield return new WaitForSeconds(chargeWindupDuration);
        if (enemyState == ChargingEnemyState.CrowdControlled ||
            enemyState == ChargingEnemyState.Dead) { charging = false; yield break; }
        //play charge animation here
        antAnimator.SetTrigger("Chasing");
        rb.velocity = chargeSpeed * direction;

        yield return new WaitForSeconds(chargeDuration);
        if (enemyState == ChargingEnemyState.CrowdControlled ||
            enemyState == ChargingEnemyState.Dead) { charging = false; yield break; }
        //play charge animation endlag here
        inChargeEndlag = true;

        yield return new WaitForSeconds(chargeEndlagDuration);
        if (enemyState == ChargingEnemyState.CrowdControlled ||
            enemyState == ChargingEnemyState.Dead) { charging = false; yield break; }
        canCharge = true;
        charging = false;
        inChargeEndlag = false;
        //If wanted, play an animation/indicator that shows that enemy can charge again
    }

    

    /*
    Every 1-3 sec, move in a randomly chosen grid-based cardinal direction
    for a random amount of time
    */
    public void IdleMove()
    {
        if (!isIdleMoving)
        {
            StartCoroutine(IdleMoveCoroutine());
        }
    }

    IEnumerator IdleMoveCoroutine()
    {
        if (hasTarget) { isIdleMoving = false; yield break; }
        if (isIdleMoving)
        {
            Debug.Log("Tried to idle move ChargingEnemy, but it was already idle moving");
            yield break;
        }

        isIdleMoving = true;
        float waitTime = UnityEngine.Random.Range(1, 3);
        int direction = (int)UnityEngine.Random.Range(0, 4);
        float moveDuration = UnityEngine.Random.Range(0, 2);
        Vector2 moveDirection = ScriptUtilities.gridDirArray[direction];

        rb.velocity = moveDirection.normalized * baseSpeed;
        yield return new WaitForSeconds(moveDuration);
        if (hasTarget) { isIdleMoving = false; yield break; }
        rb.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(waitTime);
        isIdleMoving = false;
    }

    public override void Attack(GameObject target)
    {
        Debug.Log("Attacking Player!");
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Attack(other.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //
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

    public void NotifyMeOfPlayer()
    {
        throw new System.NotImplementedException();
    }
}