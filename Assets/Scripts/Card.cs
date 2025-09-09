using UnityEngine;

// Base class instead of interface
public abstract class Card : ScriptableObject
{
    public string cardName;

    public abstract void ApplyUpgrade(Player player);
}
