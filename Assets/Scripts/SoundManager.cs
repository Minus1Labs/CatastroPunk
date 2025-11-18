using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] musicSources;
    public AudioSource[] sfxSources;
    private int currentMusicIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator PlayMusicLoop()
    {
        musicSources[currentMusicIndex].Play();
        yield return new WaitForSeconds(musicSources[currentMusicIndex].clip.length);
        currentMusicIndex = (currentMusicIndex + 1) % musicSources.Length;
        StartCoroutine(PlayMusicLoop());
    }

    // function that plays ith sfx:
    public void PlaySFX(int i)
    {
        if (i >= 0 && i < sfxSources.Length)
        {
            sfxSources[i].Play();
        }
    }
}
