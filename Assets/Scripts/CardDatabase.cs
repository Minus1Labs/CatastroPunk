using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "Cards/Card Database")]
public class CardDatabase : ScriptableObject
{
    [SerializeField] private List<Card> cards = new List<Card>();

    public Card GetRandomCard()
    {
        if (cards == null || cards.Count == 0)
            return null;

        int index = Random.Range(0, cards.Count);
        return cards[index];
    }
}
