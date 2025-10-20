using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Game/Card Definition")]
public class Card : ScriptableObject
{
    public string cardName;
    public int itemID;
    public string advancedDescription;
    public string pickupQuip;
    public int itemTier;
    public Sprite UISprite;
    public MonoScript cardBehavior;
}
