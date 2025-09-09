using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb;
    private Vector2 movementInputVector;

    public float baseSpeed = 5f;
    private float currentSpeed;

    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    private GameObject reversalProjectilePrefab;

    [SerializeField]
    private Transform firePoint;

    public int baseHealth = 5;
    private int currentHealth;

    private bool isDashing = false;
    private bool canDash = true;

    private float dashCooldown = 1f;

    public float iFrameDuration = 1f;
    private bool isInvincible = false;

    //a list of the current player card upgrades
    public List<Card> cards = new List<Card>();


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = baseSpeed;
        currentHealth = baseHealth;
        foreach (var card in cards)
        {
            card.ApplyUpgrade(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDashing) //bc dashing locks movement
        {
            SetInputVector();
        }
        // make a dash button (shift)
        if (canDash &&
        Input.GetKeyDown(KeyCode.LeftShift) &&
        movementInputVector != Vector2.zero)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetMouseButtonDown(0))
        {
            FireProjectile(projectilePrefab);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            FireProjectile(reversalProjectilePrefab);
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        currentSpeed = baseSpeed * 3;
        yield return new WaitForSeconds(0.3f);
        isDashing = false;
        currentSpeed = baseSpeed;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
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
    }

    void FireProjectile(GameObject projectile = null)
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

             GameObject go = Instantiate(projectile, firePoint.position, firePoint.rotation);
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

    void FixedUpdate()
    {
        rb.velocity = movementInputVector.normalized * currentSpeed;
    }
}
