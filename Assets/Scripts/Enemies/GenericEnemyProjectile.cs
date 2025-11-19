using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public abstract class GenericEnemyProjectile : MonoBehaviour
    {
        public float speed = 3f;
        public float projectileLifetime = 2f;
        public int damage = 1;

        [SerializeField] private bool destroyOnWall = true;
        [SerializeField] private bool destroyOnTargetHit = true;
        [SerializeField, Tooltip("0 = Hurts only player, 1 = Hurts only enemy, 2 = Hurts only interactibles/environment, 3 = Hurts all")]
        protected int projectileTeam = 0;
        protected Rigidbody2D rb;
        // Start is called before the first frame update

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            StartCoroutine(DestroyAfterTime());
        }

        public void SetVelocity(Vector2 direction, float projectileSpeed = 3f)
        {
            rb.velocity = direction.normalized * projectileSpeed;
        }

        public void SetStats(int pDamage, float projectileSpeed = 3f, float lifetime = 5f)
        {
            damage = pDamage;
            speed = projectileSpeed;
            projectileLifetime = lifetime;
        }

        //ontriggerenter2D
        void OnTriggerEnter2D(Collider2D other)
        {

            if (other.CompareTag("Wall") && destroyOnWall)
            {
                Debug.Log("THIS IS WHAT HAPPENED");
                OnProjectileRemove();
            }
            // check if other object has the IDamageable interface:
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null && CheckProjectileTeamCollision(damageable, other))
            {
                damageable.TakeDamage(damage);
                //lets do another thing if it is the ant boss
                if(damageable is AntBoss antBoss)
                {
                    antBoss.SetVulnerableState();
                }
                OnProjectileRemove();
            }
        }


        private bool CheckProjectileTeamCollision(IDamageable damageable, Collider2D other)
        {
            //TODO: Envrionment damage from enemy projectile
            bool val = projectileTeam == 3 ||
                (projectileTeam == 0 && other.CompareTag("Player")) ||
                (projectileTeam == 1 && (other.CompareTag("Enemy")));
            Debug.Log("Projectile Team: " + projectileTeam + " Other Tag: " + other.tag + " Can Damage: " + val);
            return val;
        }

        private IEnumerator DestroyAfterTime()
        {
            yield return new WaitForSeconds(projectileLifetime);
            OnProjectileRemove();
        }

        //Override this for death behavior
        public virtual void OnProjectileRemove()
        {
            Destroy(gameObject);
        }
    }
}