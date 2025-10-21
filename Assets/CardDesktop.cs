using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDesktop : MonoBehaviour
{
    CardUIManager cardUIManager;   
    // Start is called before the first frame update
    void Start()
    {
        cardUIManager = FindObjectOfType<CardUIManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cardUIManager.EnableCardUI();
            Destroy(gameObject);
        }
    }
}
