using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    void Start()
    {
        Debug.Log("HEY");
        // disable and reenable the collider on ts:
        Collider2D col = GetComponent<Collider2D>();
        col.enabled = false;
        col.enabled = true;
    }
    // on trigger enter 2d

    // ontriggerenter2d
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("wassup");
        Debug.Log("Explosion hit: " + other.tag);
        if (other.CompareTag("DoorCollider"))
        {
            LockedDoor door = other.transform.parent.GetComponentInChildren<LockedDoor>();
            if (door != null)
            {
                Debug.Log("DOOR FOUND!");
                door.SpecialInteract(ISpecialInteractible.SpecialInteractType.EXPLOSION);
            }
        } 
    }
}
