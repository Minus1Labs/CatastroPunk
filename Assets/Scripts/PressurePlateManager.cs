using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateManager : MonoBehaviour
{
    public TagPressurePlate[] pressurePlates;
    [SerializeField] private GameObject interactible;

    private bool activated = false;
    void Update()
    {
        activated = true;
        foreach (TagPressurePlate plate in pressurePlates)
        {
            if (!plate.isEnabled)
            {
                activated = false;
            }
        }

        if (activated)
        {
            interactible.GetComponent<ISpecialInteractible>().SpecialInteract(ISpecialInteractible.SpecialInteractType.PUZZLE_ONLY);
        }
    }


}
