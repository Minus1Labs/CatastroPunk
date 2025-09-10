using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardDealer : MonoBehaviour
{
    public Player player;
    [SerializeField] private CardDatabase database;

    //3 elements for the card options that the player sees on the UI:
    public GameObject[] uiCardOptions = new GameObject[3];
    
    private Card[] currentCards = new Card[3];

    void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }
    }

    public void DealCards()
    {
        currentCards[0] = database.GetRandomCard();
        currentCards[1] = database.GetRandomCard();
        currentCards[2] = database.GetRandomCard();

        // for each of the gameobjects we will
        // set active to true
        // set the textmesprougui for each object's child to the card name:
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
