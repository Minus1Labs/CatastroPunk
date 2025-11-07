using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class DialogueManager : MonoBehaviour
{
    public Dialogue dialogue;
    private int dialogueIndex;

    public TextMeshProUGUI uiText;

    public float timeBetweenChars = 0.02f;

    private bool canProceed = true;



    void Start()
    {
        dialogueIndex = 0;
    }

    void Update()
    {
        if (canProceed && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Mouse0)))
        {
            StartCoroutine(ContinueDialogue());
        }
    }

    IEnumerator ContinueDialogue()
    {
        uiText.text = "";
        canProceed = false;
        string sentence = dialogue.sentences[dialogueIndex];
        for (int i = 0; i < sentence.Length; i++)
        {
            uiText.text += sentence[i];
            yield return new WaitForSeconds(timeBetweenChars);
        }
        canProceed = true;
        dialogueIndex++;
    }

}