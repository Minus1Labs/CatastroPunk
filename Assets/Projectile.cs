using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 3f;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    Quaternion isoBase;

    void Awake()
    {
        isoBase = Quaternion.Euler(35.264f, 45f, 15f);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
    }

    void LateUpdate()
    {
        float z = transform.eulerAngles.z;
        Quaternion faceZ = Quaternion.AngleAxis(z, Vector3.forward);

        // Apply Z spin on top of the iso tilt (order matters!)
        transform.GetChild(0).rotation = isoBase * faceZ;
        // If it spins the “wrong” way, try the other order:
        // childSprite.rotation = faceZ * isoBase;
    }
    
    //ontriggerenter2
}
