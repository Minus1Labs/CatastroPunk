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
    public static GameObject[] uiCardOptions = new GameObject[3];

    public GameObject cardShopUI;
    private static Card[] currentCards = new Card[3];

    private static Card selectedCard = null;

    public static GameObject cardHighlight;

    void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }

        if (cardShopUI == null)
        {
            cardShopUI = GameObject.Find("CardShopUI");
            //under cardshop UI, there is Card1, Card2, Card3:
            uiCardOptions[0] = cardShopUI.transform.Find("Card1").gameObject;
            uiCardOptions[1] = cardShopUI.transform.Find("Card2").gameObject;
            uiCardOptions[2] = cardShopUI.transform.Find("Card3").gameObject;
        }

    }

    public void DealCards()
    {
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

        //TODO: make this less jank
        cardShopUI.transform.Find("ConfirmButton").GetComponent<Button>().onClick.AddListener(ConfirmCard);
        // cardShopUI.transform.Find("ConfirmButton (1)").GetComponent<Button>().onClick.AddListener(ConfirmCard);

        player.LockPlayer();
        selectedCard = currentCards[0];
    }

    public static void SelectCard(int index)
    {
        if (index < 0 || index >= uiCardOptions.Length)
        {
            Debug.LogError("Invalid card index selected: " + index);
            return;
        }
        // otherwise, select the card!
        selectedCard = currentCards[index];
        // find object called card highlight:
        if (cardHighlight == null)
        {
            cardHighlight = GameObject.Find("CardHighlight");
        }

        //the positions for 0, 1, 2 are (-536, -5), (-398.8, -5), (-264.3, -5):
        //this is for rect transform local position:
        cardHighlight.GetComponent<RectTransform>().anchoredPosition = new Vector3(-418 + index * 422, 0, 0);
    }

    public void ConfirmCard()
    {
        
        player.AddCard(selectedCard);
        
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
