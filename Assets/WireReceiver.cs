using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class WireReceiver : MonoBehaviour
{

    
    private void Awake()
    {
        // ensure trigger setup
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        var rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.gravityScale = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Did the wire (Recorder's EdgeCollider2D) touch me?
        // Recorder is on the same GameObject as the EdgeCollider2D
        var recorder = other.GetComponent<WireTrailRecorder>() ?? other.GetComponentInParent<WireTrailRecorder>();
        if (recorder != null)
        {
            recorder.CompleteAt(transform.position);
        }
    }
}
