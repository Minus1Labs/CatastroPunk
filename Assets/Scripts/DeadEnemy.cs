using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEnemy : MonoBehaviour, IReversible
{
    public GameObject originalEnemyObject;
    public AudioSource reverseClip;
    public float reverseVolume = 1f;

    public SpriteRenderer[] sprites;



    public void Reverse(int strength)
    {
        // Respawn original enemy
        Instantiate(originalEnemyObject, transform.position, transform.rotation);

        GetComponent<Collider2D>().enabled = false;
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].enabled = false;
        }

        // Play reverse SFX on a temp GameObject
        if (reverseClip != null)
        {
            reverseClip.Play();
        }

        // Destroy this dead enemy object immediately
        Destroy(gameObject, reverseClip.clip.length);
    }
}
