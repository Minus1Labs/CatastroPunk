using UnityEngine;

// Base class instead of interface
public abstract class Card : ScriptableObject
{
    public string cardName;

    public abstract void ApplyUpgrade(Player player);

    public virtual void OnProjectileHit(Player player) { }
    public virtual void OnEnemyKill(Player player) { }
    public virtual void OnProjectileMiss(Player player) { }
}
