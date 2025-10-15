using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class WireReceiver : MonoBehaviour
{
    [SerializeField] private AudioClip puzzleCompleteSFX;   // ?? Assign sound effect in Inspector

    private ISpecialInteractible interactible;
    private AudioSource audioSource;

    private void Awake()
    {
        // collider as trigger
        var col = GetComponent<PolygonCollider2D>();
        col.isTrigger = true;

        // 2D kinematic body
        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        // find ISpecialInteractible on self or children
        interactible = GetComponent<ISpecialInteractible>() ?? GetComponentInChildren<ISpecialInteractible>();
        if (interactible == null)
        {
            Debug.LogWarning("No ISpecialInteractible found on WireReceiver!", this);
        }

        // ?? Setup AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound
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

            // ?? Play SFX if assigned
            if (puzzleCompleteSFX != null)
            {
                audioSource.PlayOneShot(puzzleCompleteSFX);
            }
        }
    }
}
