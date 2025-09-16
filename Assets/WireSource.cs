using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireSource : MonoBehaviour
{


    // on trigger enter 2D:
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // now we want to start dragging wire from this point
            Debug.Log("Player entered wire source");
        }
    }
}
