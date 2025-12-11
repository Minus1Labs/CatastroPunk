using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class CutSceneClickThrough : MonoBehaviour
{
    Image currentImage;
    [SerializeField]
    List<Sprite> imageCutScene = new List<Sprite>();
    int next = 0;
    public string sceneToLoad;


    void Start()
    {
        currentImage = GetComponent<Image>();
        currentImage.sprite = imageCutScene[next];
    }

    // Update is called once per frame
    void Update()
    {
        if (imageCutScene!=null)
        {
            if (Input.anyKeyDown)//Input.GetKeyDown("space")
            {
                next = next + 1;
                changeImage();
            }
        }
    }

    void changeImage()
    {
        if (imageCutScene.Count == next)
        {
            SceneManager.LoadScene(sceneToLoad);
        }else 
        {
            currentImage.sprite = imageCutScene[next];
        }
    }
}
