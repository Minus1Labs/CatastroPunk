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

    public void EnableCardUI()
    {
        GenerateCards();
        cardUI.SetActive(true);
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
        CopyComponent(selectedCard.cardBehavior, player.gameObject);
        selectedCard = null;
        cardUI.SetActive(false);
    }

    public static T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        var copy = destination.AddComponent(type) as T;

        var fields = type.GetFields(System.Reflection.BindingFlags.Public |
                                    System.Reflection.BindingFlags.NonPublic |
                                    System.Reflection.BindingFlags.Instance);

        foreach (var field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }

        return copy;
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
}
