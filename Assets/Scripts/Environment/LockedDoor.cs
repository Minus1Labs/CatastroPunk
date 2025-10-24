using Unity.VisualScripting;
using UnityEngine;

public class LockedDoor : InteractibleEntity, ISpecialInteractible
{
    [Header("Door Sprites")]
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite blownOpenSprite;
    [SerializeField] private Sprite openedSprite;

    private Collider2D doorCollider = null;
    private bool is_closed = true;

    public AudioSource doorOpenSound;

    protected override void Awake()
    {
        base.Awake();

        spriteRenderer = GetComponent<SpriteRenderer>();
        Transform parentTransform = transform.parent; // get parent
        Transform sibling = parentTransform.Find("DoorCollider"); // find the sibling by name

        if (sibling != null)
        {
            // Access the sibling's GameObject or components
            GameObject siblingGO = sibling.gameObject;
            // Example: get a component
            doorCollider = siblingGO.GetComponent<Collider2D>();
        }
        else
        {
            Debug.LogWarning("DoorCollider Sibling not found!");
        }
        // Start closed
        interactible_type = InteractibleType.SPECIAL_INTERACTIBLE;
        spriteRenderer.sprite = closedSprite;
        doorCollider.enabled = true;
    }

    void Start()
    {
        
    }

    public void SpecialInteract(ISpecialInteractible.SpecialInteractType it)
    {
        if (it == ISpecialInteractible.SpecialInteractType.EXPLOSION)
        {
            spriteRenderer.sprite = blownOpenSprite;
            doorCollider.enabled = false;
            is_closed = false;
        }
        else if (it == ISpecialInteractible.SpecialInteractType.PUZZLE_ONLY)
        {
            spriteRenderer.sprite = openedSprite;
            doorCollider.enabled = false;
            is_closed = false;
            doorOpenSound.Play();
        }
    }
    public bool Is_Closed()
    {
        return is_closed;
    }
}