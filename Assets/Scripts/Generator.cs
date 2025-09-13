using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour, IReversible
{

    public GameObject barrel1;
    public GameObject barrel2;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // do the reverse action:
    public void Reverse(int strength)
    {
        // what we want to do is basically remove the current barrel next to it and 
        //enable the other barrel
        //check that they are not destroyed:
        if (barrel1 == null || barrel2 == null)
        {
            return;
        }
        barrel1.SetActive(false);
        barrel2.SetActive(true);
    }
    
    
}
