using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;
//scene management
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
    public Animator catTailAnimator;

    private Vector3 mousePos;
    public GameObject gunPos;

    private Vector2 lastAnimDir = new Vector2(1, 1);
    private Vector2 lastRawDir = Vector2.right;

    public IKManager2D currentIKManager;
    [SerializeField] public GameObject tailBone;

    public bool onlyAnimations = false;

    private bool isDashing = false;
    private bool canDash = true;
    [Header("Base Player Stats")]
    [SerializeField] public int baseHealth = 5;
    private float currentHealth;


    [SerializeField] private float baseDashCd = 1f;
    private float dashCooldown;

    public float iFrameDuration = 1f;
    private bool isInvincible = false;

    [SerializeField] private int baseDamage = 2;
    private int currentDamage;

    [SerializeField]
    private float reverseBulletCooldown;

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
    private bool chargingAmmoRefund = false;
    private bool canChargeAmmoRefund = false;
    [SerializeField, Tooltip("How long it takes to hold to refund ammo")]

    private float ammoRefundStartTime = 0f;
    private float ammoRefundAnimationTime = 3f;




    //a list of the current player card upgrades
    private List<Card> cards = new List<Card>();

    public AudioSource fireSource;
    public AudioClip fireSound;
    public AudioSource dashSound;
    public AudioSource hurtSound;
    public AudioSource reverseBulletSound;

    private Vector2 prevInput = Vector2.zero;
    private bool wasMoving = false;

    private bool canShootReverse = true;
    



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        catTailAnimator = tailBone.GetComponent<Animator>();
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

    public float GetCounter(string counterName = "Ammo")
    {
        switch (counterName)
        {
            case "Health":
                return currentHealth;
            case "Ammo":
                return currentAmmoCount;
            case "Damage":
                return currentDamage;
            default:
                Debug.LogError("Invalid counter name: " + counterName);
                return -1;
        }
    }

    public int getAmmoPerRoom()
    {
        return ammoPerRoom;
    }

    public bool CanInput()
    {
        return !isDashing && !chargingAmmoRefund;
    }


    // Update is called once per frame
    void Update()
    {
        if (CanInput()) {
            SetInputVector();
            if ((Input.GetMouseButton(0) || Input.GetKey(KeyCode.O)) && currentAmmoCount > 0)
            {
                // currentAmmoCount -= 1;
                FireProjectile();
            } else if (canShootReverse && (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.P)))
            {
                FireProjectile(true);
            }
        }

        // make a dash button (shift)
        if (canDash &&
        Input.GetKeyDown(KeyCode.LeftShift) &&
        movementInputVector != Vector2.zero)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (!chargingAmmoRefund)
            {
                Debug.Log("Starting ammo refund at time " + Time.time);
                ammoRefundStartTime = Time.time;
                chargingAmmoRefund = true;
            }
            else
            {
                if (Time.time - ammoRefundStartTime >= ammoRefundAnimationTime)
                {
                    Debug.Log("Completed ammo refund at time " + Time.time);
                    chargingAmmoRefund = false;
                    currentAmmoCount = ammoPerRoom;
                    if(currentAmmoCount == 0)
                    {
                        currentAmmoCount = 6;
                    }
                }
            }
        } else if (chargingAmmoRefund)
        {
            chargingAmmoRefund = false;
            Debug.Log("Stopped ammo refund because key stopped being held after seconds: " + (Time.time - ammoRefundStartTime));
        }

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        gunPos.transform.position = mousePos;
        // Debug.Log(gunPos.transform.position);

        //If you get an error with the above line, replace it with this:
        //mousePosition = new Vector3(mousePosition.x, mousePosition.y, zAxis);
    }

    IEnumerator Dash()
    {
        if (dashSound != null) dashSound.Play();
        canDash = false;
        isDashing = true;
        currentSpeed = baseSpeed * dashSpeed;
        isInvincible = true;
        yield return new WaitForSeconds(0.3f);
        isDashing = false;
        currentSpeed = baseSpeed;
        isInvincible = false;
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

    // Helpers to snap to your 8-way grid and measure “strength”
    private static Vector2 Snap8(Vector2 v)
    {
        return new Vector2(
            Mathf.Clamp(Mathf.Round(v.x), -1, 1),
            Mathf.Clamp(Mathf.Round(v.y), -1, 1)
        );
    }
    private static int TaxiMag(Vector2 v)
    {
        // 1 for single-axis, 2 for diagonal
        return Mathf.Abs(Mathf.RoundToInt(v.x)) + Mathf.Abs(Mathf.RoundToInt(v.y));
    }

    private void SetInputVector()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(moveHorizontal, moveVertical);
        bool isMovingNow = input.sqrMagnitude > 0.0001f;



        const float isoYScale = 0.5f; // tweak to match your tile/grid art
        Vector2 iso = new Vector2(
            input.x,
            input.y * isoYScale
        );

        movementInputVector = iso;

        // Your animator mapping (MoveX <- vertical, MoveY <- horizontal)
        float animX = moveVertical;
        float animY = moveHorizontal;

        // Decide when to update lastAnimDir (avoid diagonal→single on release)
        Vector2 curAnimDir = new Vector2(animX, animY);
        Vector2 curSnap = Snap8(curAnimDir);
        Vector2 prevAnimDir = new Vector2(prevInput.y, prevInput.x); // map prev input same way
        Vector2 prevSnap = Snap8(prevAnimDir);

        bool startedMoving = !wasMoving && isMovingNow;                 // 0 → non-zero
        bool gotStronger = TaxiMag(curSnap) > TaxiMag(prevSnap);      // single → diagonal

        if (startedMoving || (isMovingNow && gotStronger))
        {
            lastAnimDir = curSnap;
            lastRawDir = input; // keep for tail logic
        }

        // --- push animator params ---
        catBodyAnimator.SetFloat("MoveX", animX);
        catBodyAnimator.SetFloat("MoveY", animY);
        catBodyAnimator.SetFloat("LastX", lastAnimDir.x);
        catBodyAnimator.SetFloat("LastY", lastAnimDir.y);
        catBodyAnimator.SetBool("IsMoving", isMovingNow);

        // Tail keeps the last facing when stopped
        if (isMovingNow) tailChange(moveHorizontal, moveVertical);
        else tailChange(Mathf.RoundToInt(lastRawDir.x), Mathf.RoundToInt(lastRawDir.y));

        // Remember for next frame
        prevInput = input;
        wasMoving = isMovingNow;

    }

    void FireReverse()
    {
        canShootReverse = false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Plane: normal facing +Z (XY plane), passing through z = firePoint.position.z
        var plane = new Plane(Vector3.forward, new Vector3(0f, 0f, firePoint.position.z));

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hit = ray.GetPoint(enter);              // mouse world position on the XY plane
            Vector2 dir = new Vector2(hit.x - firePoint.position.x, hit.y - firePoint.position.y);
            if (dir.sqrMagnitude < 1e-6f) return; // don't shoot if too close to center
            dir.Normalize();

            // EITHER: rotate by angle (if your projectile faces +X in local space)
            firePoint.right = new Vector3(dir.x, dir.y, 0f);

            // OR (cleaner): align local +X with the direction
            // firePoint.right = dir;

            GameObject go = Instantiate(reversalProjectilePrefab, firePoint.position, firePoint.rotation);
            //shootAnimation(dir);//do not delete, will change it to a nice shoot animation
            if (reverseBulletSound != null)
            {
                reverseBulletSound.Play();
            }
        }
        StartCoroutine(ReverseCooldown());
    }

    IEnumerator ReverseCooldown()
    {
        yield return new WaitForSeconds(reverseBulletCooldown);
        canShootReverse = true;
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


            //change IK Gun bone target
            if (projectilePrefab != null && !reverse)
            {
                GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
                Projectile proj = projectileInstance.GetComponent<Projectile>();
                //shootAnimation(dir);//do not delete will change to a nice shooting anim
                proj.SetStats(currentDamage, projectileSpeed, projLifetime);
                if (fireSource != null) fireSource.PlayOneShot(fireSound);
            }
        }
    }
    void shootAnimation(Vector2 dir)
    {
        //Animation
        Vector2 invertedHit = new Vector3(-dir.x, dir.y);
        //lets rotate inverted hit 45 degrees:
        invertedHit = new Vector2(
            invertedHit.y * Mathf.Cos(-45f * Mathf.Deg2Rad) - invertedHit.y * Mathf.Sin(-45f * Mathf.Deg2Rad),
            invertedHit.x * Mathf.Sin(-45f * Mathf.Deg2Rad) + invertedHit.y * Mathf.Cos(-45f * Mathf.Deg2Rad)
        );

        if (currentIKManager != null)
        {
            (currentIKManager.solvers[0] as LimbSolver2D).GetChain(0).effector.localPosition = invertedHit;
        }
        catTailAnimator.SetTrigger("shoot");
    }
    void FixedUpdate()
    {
        if (onlyAnimations) return;
        // if not moving diagonal:
        float speedModifier = currentSpeed;
        // if (Mathf.Abs(movementInputVector.x) < 0.01f && !(Mathf.Abs(movementInputVector.y) < 0.01f) ) //only y movement
        // {
        //     speedModifier = (2.0f / Mathf.Sqrt(5)) * currentSpeed;
        // } else if (!(Mathf.Abs(movementInputVector.x) < 0.01f) && Mathf.Abs(movementInputVector.y) < 0.01f) //only x movement
        // {
        //     speedModifier = (4.0f / Mathf.Sqrt(5)) * currentSpeed;
        // }
        // Debug.Log("Speed mod: " + speedModifier);
        rb.velocity = movementInputVector.normalized * speedModifier;
    }

    void tailChange(float x, float y)//will probably make this prefabs next time
    {
        if (x == 0 && y == -1)
        {//down
            tailBone.transform.localPosition = new Vector3(0f, 0.4f, 0f);
        }
        else if (x == 0 && y == 1)
        {//up 
            tailBone.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
        else if (x == 1 && y == 0)
        {//right
            tailBone.transform.localPosition = new Vector3(-0.2f, .2f, 0f);
        }
        else if (x == 1 && y == 1 || (x == 0 && y == 0))//up right
        {
            tailBone.transform.localPosition = new Vector3(-0.2f, 0.2f, 0f);
        }
        else if (x == 1 && y == -1)//down right
        {
            tailBone.transform.localPosition = new Vector3(-0.2f, 0.3f, 0f);
        }
        else if (x == -1 && y == 0)//left
        {
            tailBone.transform.localPosition = new Vector3(0.3f, .3f, 0f);
        }
        else if (x == -1 && y == 1)//upleft
        {
            tailBone.transform.localPosition = new Vector3(0.25f, 0.2f, 0f);
        }
        else if (x == -1 && y == -1)// downleft
        {
            tailBone.transform.localPosition = new Vector3(0.2f, 0.3f, 0f);
        }

    }


    public void HitTarget()
    {
        foreach (Card c in cards)
        {
            c.OnProjectileHit(this);
        }
    }

    public void KilledEnemy()
    {
        foreach (Card c in cards)
        {
            c.OnEnemyKill(this);
        }
    }

    public void MissTarget()
    {
        foreach (Card c in cards)
        {
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
        if(isInvincible) return;
        if (hurtSound != null) hurtSound.Play();
        if (isInvincible) return;
        currentHealth -= damage;
        StartCoroutine(InvincibilityFrames());
        if (chargingAmmoRefund)
        {
            chargingAmmoRefund = false;
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeContinuousDamage(float damage)
    {
        Debug.Log("Taking continuous damage");
        if (hurtSound != null) hurtSound.Play();
        if (isInvincible) return;
        currentHealth -= damage;
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
        //TODO: more logic for player death
        // for now, we will just have it reload the scene:
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
}
