using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spikes : MonoBehaviour, IReversible
{

    public float spikeDamage = 1;

    private bool playerInSpike = false;

    private Player player;

    private bool activated = false;

    public SpriteRenderer spikesRenderer;


    void Update()
    {
        if (activated && playerInSpike)
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            }
            player.TakeDamage(spikeDamage);
        }
    }

    public void ActivateSpikes()
    {
        activated = true;
        spikesRenderer.enabled = true;
    }

    public void DeactivateSpikes()
    {
        activated = false;
        spikesRenderer.enabled = false;
    }

    public void Reverse(int direction)
    {
        DeactivateSpikes();
    }

    

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInSpike = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInSpike = false;
        }
    }
}
