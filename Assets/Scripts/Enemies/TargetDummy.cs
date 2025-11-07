using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Enemies
{
    public class TargetDummy : Enemy
    {
        public GameObject DamageTextPrefab;

        public override void Attack(GameObject target)
        {
            return;
        }

        public override void Move()
        {
            return;
        }

        public override void Reverse(int strength)
        {
            return;
        }

        public override void Die()
        {
            currentHealth = maxHealth;
        }

        public override void TakeDamage(float damage)
        {
            StartCoroutine(base.FlashWhite());
            StartCoroutine(DisplayDamageText(damage));
        }

        private IEnumerator DisplayDamageText(float damage)
        {
            Vector3 offsetAngle = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0);
            offsetAngle.Normalize();
            float offsetScale = UnityEngine.Random.Range(4f, 6f);
            GameObject textGO = Instantiate(DamageTextPrefab, transform.position + (offsetAngle * offsetScale) + DamageTextPrefab.transform.position, Quaternion.identity);
            TextMeshPro text = textGO.GetComponent<TextMeshPro>();
            text.text = ((int)damage).ToString();
            text.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            Destroy(text);
        }
    }
}