using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OnBoardPopUp : MonoBehaviour
{
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            FindObjectOfType<UIManager>().onBoardingUIActive();
        }
    }
}
