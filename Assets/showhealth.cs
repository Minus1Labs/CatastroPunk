using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showhealth : MonoBehaviour
{
    public GameObject bossHealth;

    void Start()
    {
        if (bossHealth != null)
        {
            bossHealth.SetActive(false);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (bossHealth != null)
            {
                bossHealth.SetActive(true);
            }
        }
    }
}