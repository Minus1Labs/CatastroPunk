using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public float loadInTime = 7f;
    public string sceneToLoad;

    void Start()
    {
        Invoke("LoadScene", loadInTime);
    }

    void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    
}
