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
        [SerializeField] public int cashYield = 1;

        protected float currentHealth;
        protected Rigidbody2D rb;
        protected SpriteRenderer spriteRenderer;
        protected Animator animator;

        public SpriteRenderer[] spriteRenderers; // assign in inspector
        public Material whiteFlashMaterial;       // assign the white material here
        private Material[] originalMaterials;
        public CharacterEvents characterEvents = new CharacterEvents();

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
            StartCoroutine(FlashWhite());
            CharacterDamagedEvent cde = new CharacterDamagedEvent(null, this, damage, true, ScriptUtilities.DAMAGE_TYPE.NORMAL);
            characterEvents.RaiseTakeDamage(cde);
            currentHealth -= cde.damage;
            //play animation to take damage goes here
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public float GetCurrentHealth()
        {
            return currentHealth;
        }

        public float GetMaxHP()
        {
            return maxHealth;
        }

        protected IEnumerator FlashWhite()
        {
            // swap to white flash material
            for (int i = 0; i < spriteRenderers.Length; i++)
                spriteRenderers[i].material = whiteFlashMaterial;

            yield return new WaitForSeconds(flashDuration);

            // restore original materials
            for (int i = 0; i < spriteRenderers.Length; i++)
                spriteRenderers[i].material = originalMaterials[i];
        }


        //override this for special death behavior, but be sure to include the event raise.
        //This generic does not include the explosion prefab
        public virtual void Die()
        {
            EventDispatcher.RaiseEnemyDied(new EnemyDiedEvent(this, null, this.transform.position, null));
            Destroy(gameObject);
        }

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