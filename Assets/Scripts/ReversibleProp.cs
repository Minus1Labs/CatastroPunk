using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReversibleProp : MonoBehaviour, IReversible
{

    public GameObject oldVersion;

    public void Reverse(int strength)
    {
        // what we will do is just set another object enabled and disable this one:
        if (oldVersion != null)
        {
            oldVersion.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
