using UnityEngine;

// Base class instead of interface
public abstract class Card : ScriptableObject
{
    public string cardName;
    public int itemID;
    public string advancedDescription;
    public string pickupQuip;
    public int itemTier;
    public ScriptUtilities.Item_Sprite_Info sprite_info;
    
    public abstract void ApplyUpgrade(Player player);
    public virtual void OnProjectileHit(Player player) { }
    public virtual void OnEnemyKill(Player player) { }
    public virtual void OnProjectileMiss(Player player) { }

}
