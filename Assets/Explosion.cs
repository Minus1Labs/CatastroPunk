using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    void Start()
    {
        Debug.Log("HEY");
    }
    // on trigger enter 2d

    // ontriggerenter2d
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("wassup");
    }

    //onntrigger exit2d:
    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("byee");
    }
    void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("wassup");
        Debug.Log("Explosion hit: " + other.tag);
        if (other.CompareTag("DoorCollider"))
        {
            LockedDoor door = other.transform.parent.GetComponent<LockedDoor>();
            if (door != null)
            {
                door.SpecialInteract(ISpecialInteractible.SpecialInteractType.EXPLOSION);
            }
        } 
    }
}
