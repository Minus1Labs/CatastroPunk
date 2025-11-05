using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartsUI : MonoBehaviour
{
    private Player player;
    public Image[] uiHearts;

    public Sprite emptyHeart;
    public Sprite halfHeart;
    public Sprite fullHeart;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        //use player GetCurrentHealth to update hearts UI:
        //each hp is half a heart:
        float currentHealth = player.GetCurrentHealth();
        for (int i = 0; i < uiHearts.Length; i++)
        {
            if (currentHealth >= (i + 1) * 2)
            {
                uiHearts[i].sprite = fullHeart;
            }
            else if (currentHealth == (i * 2) + 1)
            {
                uiHearts[i].sprite = halfHeart;
            }
            else
            {
                uiHearts[i].sprite = emptyHeart;
            }
        }

    }
}
