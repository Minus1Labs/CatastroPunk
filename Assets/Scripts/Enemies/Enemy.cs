using UnityEngine;



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
        [SerializeField] protected float maxHealth = 100;
        [SerializeField] protected int baseDamage = 1;

        protected float currentHealth;
        protected Rigidbody2D rb;
        protected SpriteRenderer spriteRenderer;
        protected Animator animator;

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        public virtual void TakeDamage(float damage)
        {
            currentHealth -= damage;
            //play animation to take damage goes here
            if (currentHealth <= 0)
            {
                Die();
            }
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