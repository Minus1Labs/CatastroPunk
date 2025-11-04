using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeycardSpawner : MonoBehaviour
{
    public Transform[] potentialSpawnPoints;
    public GameObject keycardPrefab;

    // Start is called before the first frame update
    void Start()
    {
        int random = Random.Range(0, potentialSpawnPoints.Length);
        Instantiate(keycardPrefab, potentialSpawnPoints[random].position, Quaternion.identity);
    }

}
