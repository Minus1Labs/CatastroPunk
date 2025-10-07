using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject onBoardingUI;
    public GameObject GameplayUI;
    //public GameObject pauseMenuUI; //for implementing it

    public static bool isGamePaused;

    // Start is called before the first frame update
    void Awake()
    {
        isGamePaused = true;
        onBoardingUI.SetActive(true);
        GameplayUI.SetActive(false);
    }

    // Update is called once per frame
    public void StartGame()
    {
        onBoardingUIDeActive();
        GameplayUI.SetActive(true);
    }

    public void onBoardingUIDeActive()
    {
        onBoardingUI.SetActive(false);
        isGamePaused = false;
    }

    public void onBoardingUIActive()
    {
        onBoardingUI.SetActive(true);
        isGamePaused = true;
        GameplayUI.SetActive(false);
    }
}
