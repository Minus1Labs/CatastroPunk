using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
[RequireComponent(typeof(Rigidbody2D))] // kinematic so triggers work reliably
public class WireTrailRecorder : MonoBehaviour
{
    private LineRenderer line;
    private EdgeCollider2D edge;
    private Rigidbody2D rb;
    private Transform player;

    private bool isRecording = false;
    private bool isLatched = false;

    private readonly List<Vector3> points = new List<Vector3>();

    [Header("Spacing")]
    public float minPointDistance = 0.2f;
    public float retractHysteresis = 0.05f;

    [Header("Appearance")]
    public float wireWidth = 0.1f;

    private float wireLength;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        edge = GetComponent<EdgeCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        line.positionCount = 0;
        line.useWorldSpace = true;
        line.startWidth = wireWidth;
        line.endWidth = wireWidth;

        // collider setup: we’ll keep it non-trigger; receivers will be triggers
        edge.edgeRadius = wireWidth * 0.5f; // fatten hitbox slightly

        // physics setup so trigger events will fire
        rb.isKinematic = true;              // kinematic rigidbody on the wire
        rb.simulated = true;
        rb.gravityScale = 0f;
    }

    public void BeginFromSource(Vector3 startPosition)
    {
        if (isLatched) return; // already finished

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (!p)
        {
            Debug.LogWarning("Recorder: No object tagged Player found.");
            return;
        }

        player = p.transform;
        points.Clear();

        // seed with start anchor + live tail
        points.Add(startPosition);
        points.Add(startPosition);

        isRecording = true;
        isLatched = false;

        PushToRenderer();
        PushToCollider();
    }

    private void Update()
    {
        if (!isRecording || isLatched || player == null || points.Count < 2) return;

        Vector3 pos = player.position;

        if (GetLineLength() <= wireLength)
        {

            // keep tail on player
            points[points.Count - 1] = pos;

            // forward extend
            Vector3 prevAnchor = points[points.Count - 2];
            float dFromPrevAnchor = Vector3.Distance(prevAnchor, pos);
            if (dFromPrevAnchor >= minPointDistance)
            {
                points.Add(pos); // new anchor + new tail
            }
        }

        // rewind (pop anchors as you backtrack)
            while (points.Count > 2)
            {
                Vector3 prePrev = points[points.Count - 3];
                Vector3 prev = points[points.Count - 2];

                float segmentLen = Vector3.Distance(prePrev, prev);
                float distToPreNow = Vector3.Distance(prePrev, pos);

                if (distToPreNow + retractHysteresis < segmentLen)
                {
                    points.RemoveAt(points.Count - 2); // pop last anchor
                }
                else break;
            }

        PushToRenderer();
        PushToCollider();
    }

    public void SetWireLength(float length)
    { 
        wireLength = length;
    }

    /// <summary>Latch the wire to a target position and stop following the player.</summary>
    public void CompleteAt(Vector3 targetWorldPos)
    {
        if (!isRecording || isLatched) return;

        // replace the live tail with a final anchor at the receiver
        if (points.Count >= 2)
        {
            points[points.Count - 1] = targetWorldPos; // tail becomes final
        }
        else
        {
            points.Clear();
            points.Add(targetWorldPos);
        }

        isLatched = true;
        isRecording = false;

        PushToRenderer();
        PushToCollider();

        Debug.Log("Recorder: Wire latched to receiver.");
    }

    public void StopRecording() => isRecording = false;

    private void PushToRenderer()
    {
        line.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
            line.SetPosition(i, FlattenZ(points[i]));
    }

    private void PushToCollider()
    {
        // EdgeCollider2D uses local-space 2D points
        if (points.Count < 2)
        {
            edge.points = System.Array.Empty<Vector2>();
            return;
        }

        var arr = new Vector2[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 world = FlattenZ(points[i]);
            Vector3 local = transform.InverseTransformPoint(world);
            arr[i] = new Vector2(local.x, local.y);
        }
        edge.points = arr;
    }

    private float GetLineLength()
    {
        if (line == null) return 0f;

        int count = line.positionCount;
        if (count < 2) return 0f;

        float total = 0f;
        Vector3 prev = line.GetPosition(0);

        for (int i = 1; i < count; i++)
        {
            Vector3 curr = line.GetPosition(i);
            total += Vector3.Distance(prev, curr);
            prev = curr;
        }

        return total;
    }


    private static Vector3 FlattenZ(Vector3 v) { v.z = 0f; return v; }
}
