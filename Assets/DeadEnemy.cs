using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEnemy : MonoBehaviour, IReversible
{
    public GameObject originalEnemyObject;
    public AudioClip reverseClip;
    public float reverseVolume = 1f;

    public void Reverse(int strength)
    {
        // Respawn original enemy
        Instantiate(originalEnemyObject, transform.position, transform.rotation);

        // Play reverse SFX on a temp GameObject
        if (reverseClip != null)
        {
            GameObject tempAudioGO = new GameObject("TempReverseSFX");
            tempAudioGO.transform.position = transform.position;

            AudioSource audioSource = tempAudioGO.AddComponent<AudioSource>();
            audioSource.clip = reverseClip;
            audioSource.volume = reverseVolume;
            audioSource.spatialBlend = 0f;
            audioSource.Play();

            Destroy(tempAudioGO, reverseClip.length);
        }

        // Destroy this dead enemy object immediately
        Destroy(gameObject);
    }
}
