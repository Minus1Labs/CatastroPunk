using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBarrel : MonoBehaviour, IDamageable
{
    public GameObject explosionEffect;
    [SerializeField] private GameObject interactibleTarget;
    public ISpecialInteractible interactible;

    public void Awake()
    {
        var source = interactibleTarget != null ? interactibleTarget : gameObject;
        interactible = source.GetComponent<ISpecialInteractible>();
    }
    public void TakeDamage(float damage)
    {

        // instantaite explosion effect and destroy self
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        //open a door
        if (interactible != null)
        {
            interactible.SpecialInteract(ISpecialInteractible.SpecialInteractType.EXPLOSION);
        }
        Destroy(gameObject);
    }
    

}
