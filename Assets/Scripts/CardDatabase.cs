using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

    //Gets (numCards) cards that are not the same. We assume that numCards is small and < cards.Count
    public List<Card> GetTBCard(int numCards)
    {
        if (numCards > 10 || numCards > cards.Count)
        {
            Debug.Log("Not enough cards in the dictionary!");
            return null;
        }

        List<int> cIndexes = new List<int>(numCards);
        for (int i = 0; i < numCards; i++)
        {
            int rand = UnityEngine.Random.Range(0, cards.Count);
            while (cIndexes.Contains(rand))
            {
                rand = UnityEngine.Random.Range(0, cards.Count);
            }
            cIndexes.Add(rand);
        }

        List<Card> ret = new List<Card>();
        for (int j = 0; j < numCards; j++)
        {
            ret.Add(cards[cIndexes[j]]);
        }
        return ret;
    }
}
