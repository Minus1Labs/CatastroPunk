using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDummyScript : MonoBehaviour, IDamageable
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void TakeDamage(int damage)
    {
        Debug.Log("Wall took " + damage + " damage!");
    }
}
