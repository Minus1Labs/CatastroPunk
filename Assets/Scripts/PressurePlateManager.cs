using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateManager : MonoBehaviour
{
    public TagPressurePlate[] pressurePlates;
    [SerializeField] private GameObject interactible;

    private bool activated = false;

    public bool doorIsOpen = false;

    void Update()
    {
        activated = true;
        foreach (TagPressurePlate plate in pressurePlates)
        {
            if (!plate.isEnabled)
            {
                activated = false;
                break;
            }
        }

        if (activated && !doorIsOpen)
        {
            interactible.GetComponent<ISpecialInteractible>().SpecialInteract(ISpecialInteractible.SpecialInteractType.PUZZLE_ONLY);
            doorIsOpen = true;
        }
    }


}
