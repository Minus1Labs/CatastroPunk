using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    public GameObject uiElements;

    public AudioSource deathScreenMusic;
    // Start is called before the first frame update
    void Start()
    {
        AudioListener.pause = false;
    }

    public void ActivateDeathScreen()
    {
        uiElements.SetActive(true);
        //time scale to 0, cease sounds, etc.
        Time.timeScale = 0f;
        AudioListener.pause = true;
        deathScreenMusic.ignoreListenerPause = true;
        
    }

    public void ResetLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
