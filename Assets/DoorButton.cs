using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public LockedDoor door;

    //ontriggerenter2d if player, call door.OpenDoor()
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision with " + collision.name);
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Door opened!");
            if (door != null)
            {
                // log "door opened"
                Debug.Log("Door opened!");
                door.SpecialInteract(ISpecialInteractible.SpecialInteractType.PUZZLE_ONLY);
            }
        }
    }
}
