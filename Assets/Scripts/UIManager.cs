using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject onBoardingUI;
    //public GameObject pauseMenuUI; //for implementing it

    public static bool isGamePaused;

    // Start is called before the first frame update
    void Awake()
    {
        isGamePaused = true;
        onBoardingUI.SetActive(true);
    }

    // Update is called once per frame
    public void StartGame()
    {
        onBoardingUI.SetActive(false);
        isGamePaused = false; 
    }
}
