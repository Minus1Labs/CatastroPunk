using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagPressurePlate : MonoBehaviour
{
    public string tagToSearch;
    [System.NonSerialized]
    public bool isEnabled = false;
    private int numEntered = 0;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.tag + " joined");
        if (other.tag == tagToSearch)
        {
            isEnabled = true;
            numEntered++;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log(other.tag + " left");
        if (other.tag == tagToSearch)
        {
            numEntered--;
            if (numEntered <= 0)
            {
                isEnabled = false;
            }
        }
    }
}
