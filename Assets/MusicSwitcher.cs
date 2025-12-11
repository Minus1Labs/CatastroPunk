using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSwitcher : MonoBehaviour
{
    public AudioSource oldSource;
    public AudioSource newSource;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            oldSource.Stop();
            newSource.Play();
        }
    }
}
