using UnityEngine;
using System.Collections;



//InteractibleEntities should implement Reversible and/or SpecialInteractible.
//Currently unenforced
namespace Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public abstract class Enemy : MonoBehaviour, IReversible, IDamageable
    {

        [Header("Stats")]
        [SerializeField] protected float baseSpeed = 2f;
        [SerializeField] protected float maxHealth = 5;
        [SerializeField] protected int baseDamage = 1;
        [SerializeField] protected int cashYield = 1;

        protected float currentHealth;
        protected Rigidbody2D rb;
        protected SpriteRenderer spriteRenderer;
        protected Animator animator;

        public SpriteRenderer[] spriteRenderers; // assign in inspector
        public Material whiteFlashMaterial;       // assign the white material here
        private Material[] originalMaterials;

        private float flashDuration = 0.1f;

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            // store original colors
            originalMaterials = new Material[spriteRenderers.Length];
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                originalMaterials[i] = spriteRenderers[i].material;
            }
        }

        public virtual void TakeDamage(float damage)
        {
            currentHealth -= damage;
            StartCoroutine(FlashWhite());
            //play animation to take damage goes here
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private IEnumerator FlashWhite()
        {
            // swap to white flash material
            for (int i = 0; i < spriteRenderers.Length; i++)
                spriteRenderers[i].material = whiteFlashMaterial;

            yield return new WaitForSeconds(flashDuration);

            // restore original materials
            for (int i = 0; i < spriteRenderers.Length; i++)
                spriteRenderers[i].material = originalMaterials[i];
        }
        //aaahh

        protected abstract void Die();

        // Movement — override in subclasses
        public abstract void Move();

        // Attack — override in subclasses
        public abstract void Attack(GameObject target);

        /*
        By default, we slow the enemy for 
        */
        public abstract void Reverse(int strength);
    }
}