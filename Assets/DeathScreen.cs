using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    public GameObject uiElements;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("YO!!!!");
        AudioListener.pause = false;
    }

    public void ActivateDeathScreen()
    {
        uiElements.SetActive(true);
        //time scale to 0, cease sounds, etc.
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void ResetLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
