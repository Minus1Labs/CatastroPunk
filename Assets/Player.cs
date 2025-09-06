using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 movementInputVector;

    private float speed = 5f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(moveHorizontal, moveVertical);

        float angle = 45f * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        movementInputVector = new Vector2(
            input.x * cos - input.y * sin,
            input.x * sin + input.y * cos
        );
    }

    void FixedUpdate()
    {
        rb.velocity = movementInputVector.normalized * speed;
    }
}
