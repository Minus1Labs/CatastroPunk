using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesActivator : MonoBehaviour
{

    public Spikes spikes;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //delay before activating spikes:
            StartCoroutine(activateInTime(0.5f));
        }
    }

    IEnumerator activateInTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        spikes.ActivateSpikes();
    }
}
