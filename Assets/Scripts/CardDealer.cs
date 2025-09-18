using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class CardDealer : MonoBehaviour
{
    public Player player;
    [SerializeField] private CardDatabase database;

    //3 elements for the card options that the player sees on the UI:
    public GameObject[] uiCardOptions = new GameObject[3];

    public GameObject cardShopUI;
    private Card[] currentCards = new Card[3];

    void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }

        if (cardShopUI == null)
        {
            cardShopUI = GameObject.Find("CardShopUI");
        }

    }

    public void DealCards()
    {
        //enable canvas component of cardShopUI:
        cardShopUI.GetComponent<Canvas>().enabled = true;

        currentCards[0] = database.GetRandomCard();
        currentCards[1] = database.GetRandomCard();
        currentCards[2] = database.GetRandomCard();

        uiCardOptions[0].GetComponent<Button>().onClick.AddListener(() => SelectCard(0));
        uiCardOptions[1].GetComponent<Button>().onClick.AddListener(() => SelectCard(1));
        uiCardOptions[2].GetComponent<Button>().onClick.AddListener(() => SelectCard(2));
        uiCardOptions[0].SetActive(true);
        uiCardOptions[0].GetComponentInChildren<TextMeshProUGUI>().text = currentCards[0].cardName;
        uiCardOptions[1].SetActive(true);
        uiCardOptions[1].GetComponentInChildren<TextMeshProUGUI>().text = currentCards[1].cardName;
        uiCardOptions[2].SetActive(true);
        uiCardOptions[2].GetComponentInChildren<TextMeshProUGUI>().text = currentCards[2].cardName;

        player.LockPlayer();
    }

    public void SelectCard(int index)
    {
        if (index < 0 || index >= uiCardOptions.Length)
        {
            Debug.LogError("Invalid card index selected: " + index);
            return;
        }
        // otherwise, select the card!
        player.AddCard(currentCards[index]);
        // now, we hide the cards and destroy this shiz:
        foreach (GameObject cardOption in uiCardOptions)
        {
            cardOption.SetActive(false);
        }
        player.UnlockPlayer();
        Destroy(gameObject);
        cardShopUI.GetComponent<Canvas>().enabled = false;
    }

    // on trigger enter2d with player, call give card:
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DealCards();
        }
    }
}
