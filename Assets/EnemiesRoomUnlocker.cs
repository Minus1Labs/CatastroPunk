using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesRoomUnlocker : MonoBehaviour
{
    //theres another thing that kinda does this, enemies manager, but using 
    //this for now
    public Transform enemiesParent;
    public GameObject linkedDoorObject;


    void Update()
    {
        if(enemiesParent.childCount == 0)
        {
            ISpecialInteractible linkedDoor = linkedDoorObject.GetComponent<ISpecialInteractible>();
            linkedDoor.SpecialInteract(ISpecialInteractible.SpecialInteractType.PUZZLE_ONLY);
            Destroy(this);
        }
    }

    
}
