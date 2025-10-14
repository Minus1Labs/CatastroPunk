using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEnemy : MonoBehaviour, IReversible
{
    
    
    public GameObject originalEnemyObject;

    public void Reverse(int strength)
    {
        Instantiate(originalEnemyObject, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    

}
