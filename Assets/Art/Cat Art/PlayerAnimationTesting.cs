using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

public class PlayerAnimationTesting : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 movementInputVector;

    private float speed = 5f;

    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    private Transform firePoint;

    //Animation 
    public Animator catBodyAnimator;

    public IKManager2D currentIKManager;
    [SerializeField]
    public GameObject tailBone;
    public Transform tailIKTarget;//probably wont need this

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        SetInputVector();
        if (Input.GetMouseButtonDown(0))
        {
            FireProjectile();
        }
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
        //change 
        tailChange(moveHorizontal, moveVertical);

    }

    void FireProjectile()
    {
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
            //move effector
            if (currentIKManager != null)
            {
                (currentIKManager.solvers[0] as LimbSolver2D).GetChain(0).effector.localPosition = invertedHit;
            }

            //simple version
            //if (tailIKTarget != null)
            //{
            //  tailIKTarget.position = gunHit;
            //}


            GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
    }

    void FixedUpdate()
    {
        rb.velocity = movementInputVector.normalized * speed;
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
}
