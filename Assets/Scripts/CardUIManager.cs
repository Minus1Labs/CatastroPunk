using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUIManager : MonoBehaviour
{
    public GameObject cardUI;

    public RectTransform cardSelect;

    public RectTransform[] cardTransforms;

    public TextMeshProUGUI[] cardUITitles;
    public TextMeshProUGUI[] cardUIDescriptions;

    [SerializeField] private CardDatabase database;

    private Card[] currentCards = new Card[3];

    private Card selectedCard = null;

    private GameObject gameplayUI;

    private AudioSource vendingSFX;
    private GameObject FloppyDisk;
    private Animator floppyDiskAnimator;

    private VendingMachineEntity myVendingMachie;

    void Start()
    {
        gameplayUI = GameObject.Find("GameplayUI");
        if(gameplayUI == null)
        {
            //search for it via whatever the parent of the instance of Hearts Ui is:
            HeartsUI heartsUI = FindObjectOfType<HeartsUI>();
            if (heartsUI != null)
            {
                gameplayUI = heartsUI.transform.parent.gameObject;
            }
        }
    }

    public void EnableCardUI()
    {
        GenerateCards();
        cardUI.SetActive(true);
        gameplayUI.SetActive(false);
    }

    public void SelectCardNumber(int cardNumber)
    {
        //set card select's position to be the same as the selected card's position, while also considering parent positions
        cardSelect.position = new Vector2(cardTransforms[cardNumber].position.x, cardSelect.position.y);
        selectedCard = currentCards[cardNumber];
    }

    public void ConfirmCard()
    {

        //add selected card's behavior to player as a component:
        Player player = FindObjectOfType<Player>();
        player.AddCard(selectedCard);

        selectedCard = null;
        cardUI.SetActive(false);
        gameplayUI.SetActive(true);

        myVendingMachie.GiveCard(player);
    }

    void GenerateCards()
    {
        for (int i = 0; i < cardTransforms.Length; i++)
        {
            currentCards[i] = database.GetRandomCard();
            cardUITitles[i].text = currentCards[i].cardName;
            cardUIDescriptions[i].text = currentCards[i].advancedDescription;
        }
        selectedCard = currentCards[0];
    }

    public void SetVendingMachine(VendingMachineEntity VendingMachine)
    {
        myVendingMachie = VendingMachine;
    }
}
