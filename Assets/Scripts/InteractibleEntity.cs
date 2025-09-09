using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]

//InteractibleEntities should implement Reversible and/or SpecialInteractible.
//Currently unenforced

//Enemies and players for now are NOT interactible entities, however in the future
//we should do something to that extent for aim snapping or objective highlighting - bm
public abstract class InteractibleEntity : MonoBehaviour
{
    [Header("Components")]
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;

    [Header("State")]
    protected InteractibleType interactible_type;

    public enum InteractibleType
    {

        REVERSIBLE,
        SPECIAL_INTERACTIBLE,
        BOTH,
        OTHER
    }

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
}
