using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keycard : MonoBehaviour
{
    
    //have a gameobject to access its ISpecialInteractible to open it:
    public GameObject interactibleToOpen;



    //ontriggerenter2d:
    private void OnTriggerEnter2D(Collider2D other)
    {
        //when player touches this open the thing then destroy self:
        if (other.CompareTag("Player"))
        {
            interactibleToOpen.GetComponent<ISpecialInteractible>().SpecialInteract(ISpecialInteractible.SpecialInteractType.PUZZLE_ONLY);
            Destroy(gameObject);
        }
    }

}
