using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeycardSpawner : MonoBehaviour
{
    public Transform[] potentialSpawnPoints;
    public GameObject keycardPrefab;

    //lets set the interactableToOpen in the Keycard script when we spawn it. we will use it from here:
    public GameObject interactibleToOpen;

    // Start is called before the first frame update
    void Start()
    {
        int random = Random.Range(0, potentialSpawnPoints.Length);
        GameObject keycard = Instantiate(keycardPrefab, potentialSpawnPoints[random].position, Quaternion.identity);
        keycard.GetComponent<Keycard>().interactibleToOpen = interactibleToOpen;
    }

}
