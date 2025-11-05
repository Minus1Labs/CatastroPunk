using Unity.Mathematics;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;
using System;

//TODO: Require 4 barrels prefabs to put on back: sealed, open-full, open-half, open-empty
public class AntBoss : Enemies.Enemy, IEnemyCommunicator
{

    [Header("Basic Enemy Properties")]
    [SerializeField] private float damageInterval = 1f; // seconds between ticks
    [SerializeField] private float detectionRadius = 12f;
    [SerializeField] private bool alertsOtherEnemies = true;

    [SerializeField] private float barrelCycleTimer = 5f;

    [SerializeField] private float chargeWindupDuration = 0.5f;
    [SerializeField] private float chargeDuration = 3f;
    [SerializeField] private float chargeSpeed = 10f;

    [SerializeField] private float throwWindupDuration = 0.5f;
    [SerializeField] private float throwWindupStartlag = 0.2f;
    [SerializeField] private float throwEndlagDuration = 0.4f;
    [SerializeField] private float chargeEndlagDuration = 1f;
    [SerializeField] private GameObject ThrownBarrelPrefab;
    [SerializeField] private GameObject ToxicPuddlePrefab;
    private float damageTimer = 0f;
    private Transform player;

    public GameObject explosionPrefab;
    //Animation
    public Animator antBossAnimator;
    //Audio! doesn't work lol
    public AudioSource audioSource;


    public enum ANT_BOSS_ENEMY_STATE
    {
        Uninitiated,
        Idle,
        Fighting,
        CrowdControlled,
        Dead
    }

    private ANT_BOSS_ENEMY_STATE enemyState;

    public List<Coroutine> coroutines = new();
    private bool charging = false;
    private bool isPassiveMoving = false;
    private bool hasTarget = false;
    private bool inChargeEndlag = false;
    private bool isBarrelAttacking = false;

    public enum ANT_BOSS_BARREL_TYPE
    {
        SEALED,
        OPEN,
        NO_BARREL
    }

    public ANT_BOSS_BARREL_TYPE[] BossCurrentBarrels = new ANT_BOSS_BARREL_TYPE[8];
    private int NextBarrelIndex = 0;
    private int openBarrelCount = 0;
    private int openBarrelUsesLeft = 4;
    private float lastBarrelUseTime = 0.0f;




    [SerializeField] private float reverseLookbackSeconds = 3f;
    [SerializeField] private float historyKeepSeconds = 2f;

    private struct PosSample { public float t; public Vector2 p; public PosSample(float t, Vector2 p) { this.t = t; this.p = p; } }
    private readonly System.Collections.Generic.LinkedList<PosSample> posHistory = new System.Collections.Generic.LinkedList<PosSample>();
    protected override void Awake()
    {
        base.Awake();
        antBossAnimator = GetComponent<Animator>();

        // Find player by tag
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
        }

        FillBossBarrels();

        enemyState = ANT_BOSS_ENEMY_STATE.Idle;

        //TODO: Decide if player should have limited reverse bullets by cooldown
        //TODO: Decide if reverse bullets should rewind boss. For now, let's say they do nothing
    }

    public void FillBossBarrels()
    {
        openBarrelCount = 0;
        for (int i = 0; i < BossCurrentBarrels.Length; i++)
        {
            int rand = (int)UnityEngine.Random.Range(0, 3);
            ANT_BOSS_BARREL_TYPE selected = ANT_BOSS_BARREL_TYPE.NO_BARREL;
            if (rand == 0)
            {
                selected = ANT_BOSS_BARREL_TYPE.SEALED;
            }
            else if (rand == 1 && openBarrelCount < 4)
            {
                openBarrelCount++;
                selected = ANT_BOSS_BARREL_TYPE.OPEN;
            }
            BossCurrentBarrels[i] = selected;
        }
        NextBarrelIndex = 0;
        openBarrelUsesLeft = 4;
    }

    public void AnimateBossRefill()
    {

        FillBossBarrels();
        //TODO: Do animation to place the barrels 
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
            if (enemyState == ANT_BOSS_ENEMY_STATE.Idle)
            {
                enemyState = ANT_BOSS_ENEMY_STATE.Fighting;
                lastBarrelUseTime = Time.time;
            }
        }

        if (enemyState == ANT_BOSS_ENEMY_STATE.Fighting)
        {
            Move();
        }

        // --- Reverse position tracking ---
        posHistory.AddLast(new PosSample(Time.time, transform.position));
        while (posHistory.Count > 1 && posHistory.First.Value.t < Time.time - historyKeepSeconds)
        posHistory.RemoveFirst();

    }
    public override void Move()
    {
        if (charging || inChargeEndlag || isBarrelAttacking) return;

        if (BossCanAttack())
        {
            BarrelCycle();
        }
        else
        {
            PassiveMove();
        }
    }

    private bool BossCanAttack()
    {
        return Time.time - lastBarrelUseTime > barrelCycleTimer;
    }


    /* Likely going to change this. 

    good: barrel attack and charge are cool

    bad: limited by barrels, feels bad to fight
    -downtime is weird
    
    */
    public void BarrelCycle()
    {
        if (NextBarrelIndex >= BossCurrentBarrels.Length)
        {
            lastBarrelUseTime = Time.time;
            return;
        }
        if (BossCurrentBarrels[NextBarrelIndex] == ANT_BOSS_BARREL_TYPE.OPEN)
        {
            //TODO: Maybe change this to do easier, more dodgable attacks and keep barrel downtime?
            //Barrel phase seems very short otherwise
            NextBarrelIndex++;
            BarrelCycle();
            return;
        }

        isBarrelAttacking = true;
        if (BossCurrentBarrels[NextBarrelIndex] == ANT_BOSS_BARREL_TYPE.SEALED)
        {
            //Start throw coroutine

            coroutines.Add(StartCoroutine(ThrowBarrelCoroutine()));

        }
        else if (BossCurrentBarrels[NextBarrelIndex] == ANT_BOSS_BARREL_TYPE.NO_BARREL)
        {
            //Start charge coroutine
            Vector2 chargeDirection = (player.position - transform.position).normalized;
            coroutines.Add(StartCoroutine(ChargeCoroutine(chargeDirection)));
        }

        lastBarrelUseTime = Time.time;
        NextBarrelIndex++;
    }

    private void ToxicWasteSplash(Vector2 direction)
    {
        if (openBarrelUsesLeft > 0)
        {
            float distanceMult = UnityEngine.Random.Range(2f, 4f) * ((rb.velocity.magnitude / 3f) + 2f);
            for (int i = 0; i < openBarrelCount; i++)
            {
                float arcOffset;
                if (openBarrelCount == 1)
                {
                    arcOffset = 0f;
                }
                else
                {
                    arcOffset = (((float)i / (openBarrelCount - 1f)) * 90f) - 45f;
                }
                Vector2 diversionAngle = Quaternion.Euler(0, 0, arcOffset) * direction;
                Vector2 puddlePosition = transform.position + (Vector3)(diversionAngle.normalized * distanceMult);
                GameObject puddle = Instantiate(ToxicPuddlePrefab, puddlePosition, Quaternion.identity);
                //Create prefab at diversionAngle distance from transform position
            }
            openBarrelUsesLeft--;
            //update sprites of the barrels on boss' back to reflect spill
        }
    }

    IEnumerator ThrowBarrelCoroutine()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        //play grabbing barrel from back animation
        antBossAnimator.SetBool("barrel", true);
        yield return new WaitForSeconds(1f);
        antBossAnimator.SetBool("barrel", false);
        rb.velocity = (direction * -1).normalized * (baseSpeed * .2f);
        yield return new WaitForSeconds(throwWindupDuration);

        rb.velocity = (direction).normalized * (baseSpeed * .5f);
        yield return new WaitForSeconds(throwWindupStartlag);
        ThrowBarrel(direction);
        ToxicWasteSplash(direction);
        yield return new WaitForSeconds(throwEndlagDuration);
        rb.velocity = new Vector2(0,0);
        isBarrelAttacking = false;
    }

    private void ThrowBarrel(Vector2 direction)
    {
        if (ThrownBarrelPrefab == null || ThrownBarrelPrefab.GetComponent<AntBossProjectile>() == null)
        {
            return;
        }
        GameObject thrownBarrel = Instantiate(ThrownBarrelPrefab, transform.position, Quaternion.identity);
        AntBossProjectile proj = thrownBarrel.GetComponent<AntBossProjectile>();
        //TODO set barrel speed as variable?
        if (proj == null || thrownBarrel == null)
        {
            return;
        }
        proj.SetVelocity(direction, ThrownBarrelPrefab.GetComponent<AntBossProjectile>().speed);
    }

    IEnumerator ChargeCoroutine(Vector2 direction)
    {
        charging = true;
        //play charge windup animation here. Moves back slightly and extends mouth blades


        rb.velocity = (direction * -1).normalized * (baseSpeed * .1f);
        antBossAnimator.SetTrigger("windup");
        yield return new WaitForSeconds(chargeWindupDuration);
        if (enemyState == ANT_BOSS_ENEMY_STATE.CrowdControlled ||
            enemyState == ANT_BOSS_ENEMY_STATE.Dead) { charging = false; yield break; }
        antBossAnimator.SetTrigger("attack");
        //play charge animation here. Quick dash with stabbing mouth blades
        rb.velocity = chargeSpeed * direction;

        yield return new WaitForSeconds(chargeDuration);

        if (enemyState == ANT_BOSS_ENEMY_STATE.CrowdControlled ||
            enemyState == ANT_BOSS_ENEMY_STATE.Dead) { charging = false; yield break; }
        //play charge animation endlag here. Boss is stunned in place after charge.
        ToxicWasteSplash(direction);
        inChargeEndlag = true;
        rb.velocity = new Vector2(0, 0);

        yield return new WaitForSeconds(chargeEndlagDuration);
        if (enemyState == ANT_BOSS_ENEMY_STATE.CrowdControlled ||
            enemyState == ANT_BOSS_ENEMY_STATE.Dead) { charging = false; yield break; }

        charging = false;
        inChargeEndlag = false;
        isBarrelAttacking = false;
        //If wanted, play an animation/indicator that shows that enemy can charge again
    }



    /*
    Every 1-3 sec, move in a randomly chosen grid-based cardinal direction
    for a random amount of time
    */
    public void PassiveMove()
    {
        if (!isPassiveMoving)
        {
            StartCoroutine(PassiveMoveCoroutine());
        }
    }

    IEnumerator PassiveMoveCoroutine()
    {
        if (isPassiveMoving)
        {
            yield break;
        }

        isPassiveMoving = true;
        float waitTime = UnityEngine.Random.Range(1, 3);
        int direction = (int)UnityEngine.Random.Range(0, 4);
        Vector2 moveDirection = ScriptUtilities.gridDirArray[direction];
        float moveTime = barrelCycleTimer - (Time.time - lastBarrelUseTime);
        rb.velocity = moveDirection.normalized * baseSpeed;

        yield return new WaitForSeconds(moveTime);
        rb.velocity = new Vector2(0, 0);
        isPassiveMoving = false;
    }

    public override void Attack(GameObject target)
    {
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

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);

        // Debug line: helps confirm if this runs
        Debug.Log("AntBoss took damage!");

        if (audioSource != null)
        {
            audioSource.Play();
        }
    }


    protected override void Die()
    {
        if (player != null)
        {
            Player pl = player.GetComponent<Player>();
            if (pl != null)
            {
                //pl.KilledEnemy();
            }
        }
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        //TODO: add any other death logic before destroying
        Destroy(gameObject);
    }

    public void NotifyMeOfPlayer()
    {
        throw new System.NotImplementedException();
    }

    public override void Reverse(int strength)
    {
        if (posHistory.Count == 0) return;

        float targetT = Time.time - Mathf.Max(0.01f, reverseLookbackSeconds);
        Vector2 targetPos;

        // find the oldest sample before the target time
        var node = posHistory.Last;
        while (node != null && node.Value.t > targetT)
            node = node.Previous;

        if (node == null)
        {
            targetPos = posHistory.First.Value.p;
        }
        else
        {
            var newer = node.Next;
            if (newer == null || Mathf.Approximately(node.Value.t, newer.Value.t))
                targetPos = node.Value.p;
            else
            {
                float u = Mathf.InverseLerp(node.Value.t, newer.Value.t, targetT);
                targetPos = Vector2.Lerp(node.Value.p, newer.Value.p, u);
            }
        }

        // --- Apply rewind ---
        rb.velocity = Vector2.zero;
        foreach (var c in coroutines)
        {
            if (c != null)
                StopCoroutine(c);
        }
        coroutines.Clear();

        rb.position = targetPos;

        while (posHistory.Count > 0 && posHistory.Last.Value.t > targetT)
            posHistory.RemoveLast();

        posHistory.AddLast(new PosSample(Time.time, rb.position));

        charging = false;
        inChargeEndlag = false;
        isBarrelAttacking = false;

        // Optional animation trigger
        if (antBossAnimator != null)
            antBossAnimator.SetTrigger("Reverse");

        Debug.Log($"AntBoss reversed to {targetPos}");
    }

}