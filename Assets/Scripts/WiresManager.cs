using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WiresManager : MonoBehaviour
{
    public WireReceiver[] wireReceivers;
    [SerializeField] private GameObject interactible;


    void Update()
    {
        bool allConnected = true;
        foreach (WireReceiver receiver in wireReceivers)
        {
            if (!receiver.activated)
            {
                allConnected = false;
                break;
            }
        }

        if (allConnected)
        {
            interactible.GetComponent<ISpecialInteractible>().SpecialInteract(ISpecialInteractible.SpecialInteractType.PUZZLE_ONLY);
            this.enabled = false;
        }
    }
}
