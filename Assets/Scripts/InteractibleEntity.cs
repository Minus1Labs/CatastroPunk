using UnityEngine;

//InteractibleEntities should implement Reversible and/or SpecialInteractible.
//Currently unenforced
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
