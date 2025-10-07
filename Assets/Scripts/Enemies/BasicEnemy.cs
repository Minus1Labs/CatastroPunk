using Unity.Mathematics;
using UnityEngine;
using System.Collections;

public class BasicEnemy : Enemies.Enemy
{

    [Header("Basic Enemy Properties")]
    [SerializeField] private float damageInterval = 1f; // seconds between ticks
    [SerializeField] private float detectionRadius = 6f;

    private float damageTimer = 0f;
    private Transform player;

    public GameObject explosionPrefab;

    public Animator robotAnimator;

    public AudioSource attackSound;
    public AudioSource deathSound;

    [SerializeField] private float reverseLookbackSeconds = 3f;
    [SerializeField] private float historyKeepSeconds = 2f;

    private struct PosSample { public float t; public Vector2 p; public PosSample(float t, Vector2 p){ this.t=t; this.p=p; } }
    private readonly System.Collections.Generic.LinkedList<PosSample> posHistory = new System.Collections.Generic.LinkedList<PosSample>();

    private float currentEnemyTime = 0f;

    private bool isShootingPlayer = true, canShoot = true;
    [SerializeField] private float shootInterval = 1f;
    [SerializeField] private GameObject bulletPrefab;

    protected override void Awake()
    {
        base.Awake();

        // Find player by tag
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
        }

        posHistory.Clear();
        posHistory.AddLast(new PosSample(Time.time, transform.position));
        currentEnemyTime = Time.time;
    }

    private void Update()
    {
        currentEnemyTime += Time.deltaTime;
        if (player == null) return;
        if (UIManager.isGamePaused)return;
        
        if (canShoot && isShootingPlayer)
        {
            ShootPlayer();
            canShoot = false;
            StartCoroutine(ResetShoot());
        }

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
        posHistory.AddLast(new PosSample(currentEnemyTime, transform.position));
        // float cutoff = Time.time - Mathf.Max(historyKeepSeconds, reverseLookbackSeconds + 0.25f);
        // while (posHistory.Count > 1 && posHistory.First.Value.t < cutoff)
        // posHistory.RemoveFirst();
        while (posHistory.Count > 1 && posHistory.First.Value.t < currentEnemyTime - historyKeepSeconds)
            posHistory.RemoveFirst();
    }

    void ShootPlayer()
    {

        // basically get player position, and we will instantiate a bullet prefab
        //that points towards the player:
        if (player == null || bulletPrefab == null) return;

        Vector2 shootDir = (player.position - transform.position).normalized;

        //simply instantiate the bullet prefab with a rotation that points towards the player:
        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);
        Instantiate(bulletPrefab, transform.position, rot);
    }

    IEnumerator ResetShoot()
    {
        yield return new WaitForSeconds(shootInterval);
        canShoot = true;
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

            if (attackSound != null) attackSound.Play();
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

        robotAnimator.SetTrigger("Die");

        if (deathSound != null)
        {
            GameObject deathSoundGO = new GameObject("DeathSound");
            AudioSource audioSource = deathSoundGO.AddComponent<AudioSource>();
            audioSource.clip = deathSound.clip;
            audioSource.volume = deathSound.volume;
            audioSource.spatialBlend = deathSound.spatialBlend;
            audioSource.Play();


            Destroy(deathSoundGO, deathSound.clip.length);
        }

        Destroy(gameObject); 
    }





    public override void Reverse(int strength)
{
    Debug.Log(posHistory.Count);
    float lookback = reverseLookbackSeconds; // or scale by strength if desired
    float targetT = currentEnemyTime - Mathf.Max(0.01f, lookback);
    targetT = Mathf.Max(0f, targetT);
    Debug.Log("Target T: " + targetT);
    currentEnemyTime -= lookback;
    currentEnemyTime = Mathf.Max(0f, currentEnemyTime);

    if (posHistory.Count == 0) return;

    // Find the sample node <= targetT (scan from newest backward)
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
        var newer = node.Next;
        if (newer == null || Mathf.Approximately(node.Value.t, newer.Value.t))
            targetPos = node.Value.p;
        else
        {
            float u = Mathf.InverseLerp(node.Value.t, newer.Value.t, targetT);
            targetPos = Vector2.Lerp(node.Value.p, newer.Value.p, u);
        }

        robotAnimator.SetTrigger("Reverse");
        // Teleport + stop motion
        rb.position = targetPos;
        rb.velocity = Vector2.zero;

        // (Optional tiny cleanup) write a fresh sample now so history stays consistent
        posHistory.AddLast(new PosSample(Time.time, rb.position));

    }

    // --- TRUNCATE FUTURE SAMPLES: remove any samples with t > targetT ---
    while (posHistory.Count > 0 && posHistory.Last.Value.t > targetT)
        posHistory.RemoveLast();

    // Teleport & stop
    rb.position = targetPos;
    rb.velocity = Vector2.zero;

    // Add a fresh sample at the CURRENT time/position (this becomes the new tail)
    posHistory.AddLast(new PosSample(Time.time, rb.position));
}

}