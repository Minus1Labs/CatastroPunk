using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartsUI : MonoBehaviour
{
    private Player player;

    public Sprite emptyHeart;
    public Sprite halfHeart;
    public Sprite fullHeart;

    public GameObject uiHeartPrefab;

    public bool dynamicallyGeneratedHearts = false;
    
    
    
    
    //EXPLICITLY SET THIS, IF DYNAMICALLY GENERATED IS FALSE. make that a tooltiP:

    
    public Image[] uiHearts;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        InitializeHeartsUI();
    }

    void InitializeHeartsUI()
    {
        if(!dynamicallyGeneratedHearts)
        {
            return;
        }
        int maxHealth = player.GetMaxHP();
        int totalHearts = Mathf.CeilToInt(maxHealth / 2f);
        uiHearts = new Image[totalHearts];

        float spacing = 5f; //spacing between hearts
        GameObject sampleHeart = uiHeartPrefab;
        RectTransform sampleRT = sampleHeart.GetComponent<RectTransform>();
        float heartWidth = sampleRT.sizeDelta.x;

        // Calculate total width of all hearts + spacing
        float totalWidth = totalHearts * heartWidth + (totalHearts - 1) * spacing;
        float startX = -totalWidth / 2f + heartWidth / 2f; // center origin

        for (int i = 0; i < totalHearts; i++)
        {
            GameObject heart = Instantiate(uiHeartPrefab, transform);
            RectTransform rt = heart.GetComponent<RectTransform>();

            float x = startX + i * (heartWidth + spacing);
            rt.anchoredPosition = new Vector2(x, 0);

            uiHearts[i] = heart.GetComponent<Image>();
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (uiHearts == null)
        {
            InitializeHeartsUI();
        }
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
