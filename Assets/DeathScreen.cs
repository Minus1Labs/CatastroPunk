using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    public GameObject uiElements;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ActivateDeathScreen()
    {
        uiElements.SetActive(true);
    }
}
