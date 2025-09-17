using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WireSource : MonoBehaviour
{
    public WireTrailRecorder recorder;   // assign in Inspector
    private Transform player;             // your player Transform

    public float wireLength = 10f;

    void Start()
    { 
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }
    void Reset()
    {
        var col = GetComponent<PolygonCollider2D>();
        if (col) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!player) return;
        if (collision.transform == player || collision.CompareTag("Player"))
        {
            // Start tracing from THIS source and keep going even after we leave
            recorder?.BeginFromSource(transform.position);
            recorder?.SetWireLength(wireLength);
            Debug.Log("Wire armed: recording started");
        }
    }

}
