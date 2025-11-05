using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;
//scene management
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Unity.Collections;
using JetBrains.Annotations;
using UnityEngine.Android;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb;
    private Vector2 movementInputVector;

    //public float baseSpeed = 5f;
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
    


    [SerializeField] private float baseDashCd = 4.0f;
    private float dashCooldown;

    public float iFrameDuration = 1f;
    private bool isInvincible = false;

    [SerializeField] private float reverseBulletCooldown = 1.5f;

    private float reverseCooldownValue = 1f; //this is what we use to fill up the cooldown ui bar:

    [SerializeField] private float baseProjectileSpeed = 12f;
    private float projectileSpeed;

    [SerializeField] private float baseDashSpeed = 3f;
    private float dashSpeed;


    [SerializeField] private float baseProjectileLifetime = 5f;
    private float projLifetime;

    //a list of the current player card upgrades
    public PlayerStats baseStats;
    public PlayerStats additiveStats;
    public PlayerStats multStats;
    public int cash;
    private int currentHealth;
    public float currentCharge;


    public AudioSource fireSource;
    public AudioClip fireSound;
    public AudioSource dashSound;
    public AudioSource hurtSound;
    public AudioSource reverseBulletSound;

    private Vector2 prevInput = Vector2.zero;
    private bool wasMoving = false;

    private bool canShootReverse = true;

    private bool canFire = true;

    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector2 aimInput;
    private bool usingGamepad;




    void Awake()
    {
        controls = new PlayerControls();

        controls.Gameplay.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += _ => moveInput = Vector2.zero;

        controls.Gameplay.Aim.performed += ctx =>
        {
            aimInput = ctx.ReadValue<Vector2>();
            usingGamepad = ctx.control.device is Gamepad;
        };
        controls.Gameplay.Aim.canceled += _ => aimInput = Vector2.zero;

        // Shooting
        // --- Shooting ---
        controls.Gameplay.Fire1.performed += _ =>
        {
            if (CanInput() && currentCharge >= GetShotEnergy() && canFire)
            {
                currentCharge -= GetShotEnergy();
                canFire = false;
                FireProjectile();
                StartCoroutine(EnableFire());
            }
        };

        controls.Gameplay.Fire2.performed += _ =>
        {
            if (CanInput() && canShootReverse)
                FireProjectile(true);
        };

        // --- Dash ---
        controls.Gameplay.Dash.performed += _ =>
        {
            if (CanInput() && canDash && movementInputVector != Vector2.zero)
                StartCoroutine(Dash());
        };

    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        catTailAnimator = tailBone.GetComponent<Animator>();
        projectileSpeed = baseProjectileSpeed;
        dashSpeed = baseDashSpeed;
        projLifetime = baseProjectileLifetime;

        baseStats = ScriptableObject.CreateInstance<PlayerStats>();
        additiveStats = ScriptableObject.CreateInstance<PlayerStats>();
        additiveStats.SetAllNum(0);
        multStats = ScriptableObject.CreateInstance<PlayerStats>();
        multStats.SetAllNum(1);

        currentHealth = baseStats.health;
        currentSpeed = GetMoveSpeed();
        currentCharge = GetBatteryCapacity();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();


    public void AddCard(Card c)
    {
        if (c == null)
        {
            Debug.LogError("Attempted to add card, but object is null");
        }

        if (c.BehaviourType == null)
        {
            Debug.LogError("Attempted to add card, but CardBehavior attribute of Card is null");
        }

        CardBehavior childComponent = GetComponent(c.BehaviourType) as CardBehavior;
        if (childComponent != null)
        {
            childComponent.AddStack(this);
        }
        else
        {
            CardBehavior cb = gameObject.AddComponent(c.BehaviourType) as CardBehavior;
            cb.ApplyUpgrade(this);
        }
        // TODO: send out signal OR directly call UI to update item UI
        // TODO: send out signal to rigging to update visuals
    }

    public float GetDamage()
    {
        return (baseStats.damage + additiveStats.damage) * multStats.damage;
    }

    public float GetMoveSpeed()
    {
        return (baseStats.moveSpeed + additiveStats.moveSpeed) * multStats.moveSpeed;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHP()
    {
        return (baseStats.health + additiveStats.health) * multStats.health;
    }

    public float GetDashCd()
    {
        return (baseStats.dashCooldown + additiveStats.dashCooldown) * multStats.dashCooldown;
    }

    //public float GetIFrameDur() {}

    public float GetBatteryCapacity()
    {
        return (baseStats.batteryCapacity + additiveStats.batteryCapacity) * multStats.batteryCapacity;
    }

    public float GetCurrentCharge()
    {
        return currentCharge;
    }

    public float GetEnergyRegain() {
        return (baseStats.energyRegain + additiveStats.energyRegain) * multStats.energyRegain;
    }

    public float GetFirerate()
    {
        return (baseStats.shotsPerSecond + additiveStats.shotsPerSecond) * multStats.shotsPerSecond;
    }

    public float GetShotEnergy()
    {
        return (baseStats.energyPerShot + additiveStats.energyPerShot) * multStats.energyPerShot;
    }

    public float GetLuck()
    {
        return (baseStats.luck + additiveStats.luck) * multStats.luck;
    }

    public float GetReverseCooldownValue()
    {
        return reverseCooldownValue;
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

    public bool CanInput()
    {
        return !isDashing;
    }

    IEnumerator EnableFire()
    {
        yield return new WaitForSeconds(1/GetFirerate());
        canFire = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (!CanInput()) return;

        SetInputVector();  // keep your movement + animation system

        if (usingGamepad)
        {
            
        }
        else
        {
            Vector3 mouseScreen = new Vector3(aimInput.x, aimInput.y, 0f);
            Vector3 world = Camera.main.ScreenToWorldPoint(mouseScreen);
            world.z = 0;
            gunPos.transform.position = world;
        }
    }


    IEnumerator Dash()
    {
        if (dashSound != null) dashSound.Play();
        canDash = false;
        isDashing = true;
        currentSpeed = GetMoveSpeed() * dashSpeed;
        isInvincible = true;
        yield return new WaitForSeconds(0.3f);
        isDashing = false;
        currentSpeed = GetMoveSpeed();
        isInvincible = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
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
        Vector2 input = moveInput; // value from new Input System

        const float isoYScale = 0.5f; // tweak to match your tile/grid art
        Vector2 iso = new Vector2(input.x, input.y * isoYScale);
        movementInputVector = iso;

        bool isMovingNow = input.sqrMagnitude > 0.0001f;

        // animator mapping (MoveX <- vertical, MoveY <- horizontal)
        float animX = input.y;
        float animY = input.x;

        Vector2 curAnimDir = new Vector2(animX, animY);
        Vector2 curSnap = Snap8(curAnimDir);
        Vector2 prevAnimDir = new Vector2(prevInput.y, prevInput.x);
        Vector2 prevSnap = Snap8(prevAnimDir);

        bool startedMoving = !wasMoving && isMovingNow;
        bool gotStronger = TaxiMag(curSnap) > TaxiMag(prevSnap);

        if (startedMoving || (isMovingNow && gotStronger))
        {
            lastAnimDir = curSnap;
            lastRawDir = input;
        }

        catBodyAnimator.SetFloat("MoveX", animX);
        catBodyAnimator.SetFloat("MoveY", animY);
        catBodyAnimator.SetFloat("LastX", lastAnimDir.x);
        catBodyAnimator.SetFloat("LastY", lastAnimDir.y);
        catBodyAnimator.SetBool("IsMoving", isMovingNow);

        if (isMovingNow) tailChange(input.x, input.y);
        else tailChange(Mathf.RoundToInt(lastRawDir.x), Mathf.RoundToInt(lastRawDir.y));

        prevInput = input;
        wasMoving = isMovingNow;
    }


    void FireReverse()
    {
        canShootReverse = false;
        Vector2 screenPos = usingGamepad
    ? (Vector2)Camera.main.WorldToScreenPoint(firePoint.position + (Vector3)aimInput * 10f)
    : aimInput;

    Ray ray = Camera.main.ScreenPointToRay(screenPos);


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
            Projectile proj = go.GetComponent<Projectile>();
            proj.SetStats(0, projectileSpeed, projLifetime);
            
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
        reverseCooldownValue = 0f;
        float cooldownTime = reverseBulletCooldown;
        while (reverseCooldownValue < 1f)
        {
            reverseCooldownValue += Time.deltaTime / cooldownTime;
            reverseCooldownValue = Mathf.Clamp01(reverseCooldownValue);
            yield return null;
        }
        canShootReverse = true;
    }

    void FireProjectile(bool reverse = false)
    {
        //Scuffed btw
        if (reverse)
        {
            FireReverse();
        }


        Vector2 screenPos = usingGamepad
    ? (Vector2)Camera.main.WorldToScreenPoint(firePoint.position + (Vector3)aimInput * 10f)
    : aimInput;

        Ray ray = Camera.main.ScreenPointToRay(screenPos);


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
                proj.SetStats(GetDamage(), projectileSpeed, projLifetime);
                if (fireSource != null)
                {
                    fireSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                    fireSource.PlayOneShot(fireSound);
                }
            }
        }
    }
    void ShootAnimation(Vector2 dir)
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
        float speedModifier = currentSpeed * 7.0f;
        if (currentCharge < GetBatteryCapacity() && canFire)
        {
            currentCharge += GetEnergyRegain() * Time.fixedDeltaTime;
            currentCharge = currentCharge > GetBatteryCapacity() ?
                GetBatteryCapacity() : currentCharge;
        }
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

    public void AddHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > GetMaxHP())
        {
            currentHealth = GetMaxHP();
        }
    }

    public void TakeDamage(float damage)
    {

        if (isInvincible) return;
        if (hurtSound != null) hurtSound.Play();
        if (isInvincible) return;
        currentHealth -= (int)damage;
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
        //TODO: more logic for player death
        // for now, we will just have it reload the scene:
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
}
