using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieAfterTime : MonoBehaviour
{
    public float lifetime = 3f;
    public AudioSource explosionAudioSource;

    void Start()
    {
        if (explosionAudioSource != null)
        {
            explosionAudioSource.Play();
        }

        Destroy(gameObject, lifetime);
    }
}
