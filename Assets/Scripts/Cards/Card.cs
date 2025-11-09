using UnityEditor;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card Definition")]
public class Card : ScriptableObject
{
    public string cardName;
    public int itemID;
    public string advancedDescription;
    public string pickupQuip;
    public int itemTier;
    public Sprite UISprite;

    [SerializeField] private string behaviorTypeName;

    public Type BehaviourType => Type.GetType(behaviorTypeName);
    //public MonoBehaviour cardBehavior;
}
