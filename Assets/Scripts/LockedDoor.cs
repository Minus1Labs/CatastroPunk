using Unity.VisualScripting;
using UnityEngine;

public class LockedDoor : InteractibleEntity, ISpecialInteractible
{
    [Header("Door Sprites")]
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite blownOpenSprite;
    [SerializeField] private Sprite openedSprite;

    private Collider2D doorCollider;
    private bool is_closed = true;

    protected override void Awake()
    {
        base.Awake();

        spriteRenderer = GetComponent<SpriteRenderer>();
        doorCollider = GetComponent<Collider2D>();

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
        }
    }
    public bool Is_Closed()
    {
        return is_closed;
    }
}