using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class WireReceiver : MonoBehaviour
{
    [SerializeField] private GameObject interactibleTarget; // drag the target object here (or leave null to use self)

    private ISpecialInteractible interactible;

    private void Awake()
    {
        // collider as trigger
        var col = GetComponent<PolygonCollider2D>();
        col.isTrigger = true;

        // 2D kinematic body
        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic; // (prefer over isKinematic in 2D)
        rb.gravityScale = 0f;

        // resolve the interface component at runtime
        var source = interactibleTarget != null ? interactibleTarget : gameObject;
        interactible = source.GetComponent<ISpecialInteractible>();
        if (interactible == null)
        {
            // find the component in children:
            interactible = source.GetComponentInChildren<ISpecialInteractible>();
            if (interactible == null)
            {
                Debug.LogWarning("No ISpecialInteractible found on WireReceiver target!", this);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var recorder = other.GetComponent<WireTrailRecorder>() ?? other.GetComponentInParent<WireTrailRecorder>();
        if (recorder != null)
        {
            recorder.CompleteAt(transform.position);
            if (interactible != null) 
            {
                interactible.SpecialInteract(ISpecialInteractible.SpecialInteractType.PUZZLE_ONLY);
            }
        }

        
    }
}
