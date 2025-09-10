using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

public class Player : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb;
    private Vector2 movementInputVector;

    public float baseSpeed = 5f;
    private float currentSpeed;

    [Header("Required Setup")]

    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    private GameObject reversalProjectilePrefab;

    [SerializeField]
    private Transform firePoint;

    public Animator catBodyAnimator;

    public IKManager2D currentIKManager;
    [SerializeField] public GameObject tailBone;
    public Transform tailIKTarget;

    public bool onlyAnimations = false;

    
    private bool isDashing = false;
    private bool canDash = true;
    [Header("Base Player Stats")]
    [SerializeField] public int baseHealth = 5;
    private int currentHealth;


    [SerializeField] private float baseDashCd = 1f;
    private float dashCooldown;

    public float iFrameDuration = 1f;
    private bool isInvincible = false;

    [SerializeField] private int baseDamage = 80;
    private int currentDamage;

    [SerializeField] private float baseProjectileSpeed = 12f;
    private float projectileSpeed;

    [SerializeField] private float baseDashSpeed = 3f;
    private float dashSpeed;

    [SerializeField] private int baseAmmoPerRoom = 6;
    private int ammoPerRoom;

    [SerializeField] private float baseRefundTime = 10f;
    private float refundTime;

    [SerializeField] private float baseProjectileLifetime = 5f;
    private float projLifetime;

    private int currentAmmoCount;

    private bool canRefund = true;



    //a list of the current player card upgrades
    private List<Card> cards = new List<Card>();

    public AudioSource fireSound;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = baseSpeed;
        currentHealth = baseHealth;
        currentDamage = baseDamage;
        projectileSpeed = baseProjectileSpeed;
        dashSpeed = baseDashSpeed;
        dashCooldown = baseDashCd;
        ammoPerRoom = baseAmmoPerRoom;
        currentAmmoCount = ammoPerRoom;
        refundTime = baseRefundTime;
        projLifetime = baseProjectileLifetime;
        foreach (var card in cards)
        {
            card.ApplyUpgrade(this);
        }
    }

    public void AddCard(Card c)
    {
        if (c != null)
        {
            c.ApplyUpgrade(this);
            cards.Add(c);
        }
    }

    public void SetDamage(float multiplier = 1f, int additive = 0, int setTo = -1)
    {
        currentDamage = (int)(currentDamage * multiplier) + additive;
        if (setTo >= 0)
        {
            currentDamage = setTo;
        }
    }

    public void SetSpeed(float multiplier = 1f, float additive = 0f, float setTo = -1f)
    {
        currentSpeed = currentSpeed * multiplier + additive;
        if (setTo >= 0)
        {
            currentSpeed = setTo;
        }
    }

    public void SetHealth(float multiplier = 1f, int additive = 0, int setTo = -1)
    {
        currentHealth = (int)(currentHealth * multiplier) + additive;
        if (setTo > 0)
        {
            currentHealth = setTo;
        }
    }

    public void SetDashCd(float multiplier = 1f, float additive = 0f, float setTo = -1f)
    {
        dashCooldown = dashCooldown * multiplier + additive;
        if (setTo >= 0)
        {
            dashCooldown = setTo;
        }
    }

    public void SetIFrameDur(float multiplier = 1f, float additive = 0f, float setTo = -1f)
    {
        iFrameDuration = iFrameDuration * multiplier + additive;
        if (setTo >= 0)
        {
            iFrameDuration = setTo;
        }
    }

    public void SetAmmoPerRoom(float multiplier = 1f, int additive = 0, int setTo = -1)
    {
        ammoPerRoom = (int)(ammoPerRoom * multiplier) + additive;
        if (setTo >= 0)
        {
            ammoPerRoom = setTo;
        }
    }

    public void SetCurrentAmmo(float multiplier = 1f, int additive = 0, int setTo = -1)
    {
        currentAmmoCount = (int)(currentAmmoCount * multiplier) + additive;
        if (setTo >= 0)
        {
            currentAmmoCount = setTo;
        }
    }

    public void LockPlayer()
    {
        //cant move nor shoot:
        this.enabled = false;
        // set rb velocity to 0:
        rb.velocity = Vector2.zero;
    }

    public void UnlockPlayer()
    {
        this.enabled = true;
    }

    public int GetCurrentAmmoCount()
    {
        return currentAmmoCount;
    }

    public int getAmmoPerRoom()
    {
        return ammoPerRoom;
    }


    // Update is called once per frame
    void Update()
    {
        if (!isDashing) //bc dashing locks movement
        {
            SetInputVector();
        }
        if (Input.GetMouseButtonDown(0) && currentAmmoCount > 0 && !isDashing)
        {
            Debug.Log("SHOOTING " + currentAmmoCount);
            currentAmmoCount -= 1;
            FireProjectile();
        }

        // make a dash button (shift)
        if (canDash &&
        Input.GetKeyDown(KeyCode.LeftShift) &&
        movementInputVector != Vector2.zero)
        {
            StartCoroutine(Dash());
        }

        if (canRefund && currentAmmoCount < ammoPerRoom)
        {
            StartCoroutine(RefundAmmo());
        }

        else if (Input.GetMouseButtonDown(1) && !isDashing)
        {
            FireProjectile(true);
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        currentSpeed = baseSpeed * dashSpeed;
        yield return new WaitForSeconds(0.3f);
        isDashing = false;
        currentSpeed = baseSpeed;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    IEnumerator RefundAmmo()
    {
        canRefund = false;
        yield return new WaitForSeconds(refundTime);
        currentAmmoCount += 1;
        canRefund = true;
    }

    private void SetInputVector()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(moveHorizontal, moveVertical);

        float angle = 45f * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        // Rotate input vector by 45 degrees to align with isometric view
        movementInputVector = new Vector2(
            input.x * cos - input.y * sin,
            input.x * sin + input.y * cos
        );

        //Animation
        catBodyAnimator.SetFloat("MoveX", moveVertical);
        catBodyAnimator.SetFloat("MoveY", moveHorizontal);
        tailChange(moveHorizontal, moveVertical);
    }

    void FireReverse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Plane: normal facing +Z (XY plane), passing through z = firePoint.position.z
            var plane = new Plane(Vector3.forward, new Vector3(0f, 0f, firePoint.position.z));

            if (plane.Raycast(ray, out float enter))
            {
                Vector3 hit = ray.GetPoint(enter);              // mouse world position on the XY plane
                Vector2 dir = new Vector2(hit.x - firePoint.position.x, hit.y - firePoint.position.y);
                if(dir.sqrMagnitude < 1e-6f) return; // don't shoot if too close to center
                dir.Normalize();

                // EITHER: rotate by angle (if your projectile faces +X in local space)
                firePoint.right = new Vector3(dir.x, dir.y, 0f);

                // OR (cleaner): align local +X with the direction
                // firePoint.right = dir;

                GameObject go = Instantiate(reversalProjectilePrefab, firePoint.position, firePoint.rotation);
                if(fireSound != null) fireSound.Play();
            }
    }

    void FireProjectile(bool reverse = false)
    {
        //Scuffed btw
        if (reverse)
        {
            FireReverse();
        }


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Plane: normal facing +Z (XY plane), passing through z = firePoint.position.z
        var plane = new Plane(Vector3.forward, new Vector3(0f, 0f, firePoint.position.z));

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hit = ray.GetPoint(enter);              // mouse world position on the XY plane
            Vector2 dir = (hit - firePoint.position);       // 2D direction (x,y)

            dir.Normalize();

            // EITHER: rotate by angle (if your projectile faces +X in local space)
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            firePoint.rotation = Quaternion.Euler(0f, 0f, angle);

            // OR (cleaner): align local +X with the direction
            // firePoint.right = dir;

            //Animation
            Vector2 invertedHit = new Vector3(-dir.x, dir.y);
            //lets rotate inverted hit 45 degrees:
            invertedHit = new Vector2(
                invertedHit.x * Mathf.Cos(-45f * Mathf.Deg2Rad) - invertedHit.y * Mathf.Sin(-45f * Mathf.Deg2Rad),
                invertedHit.x * Mathf.Sin(-45f * Mathf.Deg2Rad) + invertedHit.y * Mathf.Cos(-45f * Mathf.Deg2Rad)
            );
            //change IK Gun bone target
            if (currentIKManager != null)
            {
                (currentIKManager.solvers[0] as LimbSolver2D).GetChain(0).effector.localPosition = invertedHit;
            }

            //simple version
            //if (tailIKTarget != null)
            //{
            //  tailIKTarget.position = gunHit;
            //}

            if (projectilePrefab != null && !reverse)
            {
                GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
                Projectile proj = projectileInstance.GetComponent<Projectile>();

                proj.SetStats(currentDamage, projectileSpeed, projLifetime);
                if(fireSound != null) fireSound.Play();
            }
        }
    }

    void FixedUpdate()
    {
        if(onlyAnimations) return;
        rb.velocity = movementInputVector.normalized * currentSpeed;
    }

    void tailChange(float x, float y)//will probably make this prefabs next time
    {
       
        if ((x == 0 && y == -1) || (x == 0 && y == 1)) 
        {//up & down
            tailBone.transform.localPosition = new Vector3(-0.2f, 0.11f, -0.1f);
        }
        else if (x == 1 && y == 0)
        {//right
            tailBone.transform.localPosition = new Vector3(-2.4f, 0.1f, -0.1f);
        }
        else if (x == 1 && y == 1 || (x == 0 && y == 0))//up right
{
            tailBone.transform.localPosition = new Vector3(-1.84f, -0.5f, -0.1f);
        }
        else if (x == 1 && y == -1)//down right
 {
            tailBone.transform.localPosition = new Vector3(-1.6f, 0.11f, -0.1f);
        }
        else if (x == -1 && y == 0)//left
 {
            tailBone.transform.localPosition = new Vector3(3f, 0.1f, -0.1f);
        }
        else if (x == -1 && y == 1)//upleft
{
            tailBone.transform.localPosition = new Vector3(2.5f, -1f, -0.1f);
        }
        else if (x == -1 && y == -1)// downleft
        {
            tailBone.transform.localPosition = new Vector3(2f, 0.1f, -0.1f);
        }
    }

    public void HitTarget() {
        foreach (Card c in cards) {
            c.OnProjectileHit(this);
        }
    }

    public void KilledEnemy() {
        foreach (Card c in cards) {
            c.OnEnemyKill(this);
        }
    }

    public void MissTarget() {
        foreach (Card c in cards) {
            c.OnProjectileMiss(this);
        }
    }


    
    public void AddHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > baseHealth)
        {
            currentHealth = baseHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("TAKING DAMAGE");
        if (isInvincible) return;
        currentHealth -= damage;
        StartCoroutine(InvincibilityFrames());
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        //make the player flash from opacity 0 to 1 for iframes:
        for (float i = 0; i < iFrameDuration; i += 0.1f)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.05f);
            GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.05f);
        }
        isInvincible = false;
    }

    private void Die()
    {
        Debug.Log("Player died!");
        //TODO: more logic for player death
    }
}
