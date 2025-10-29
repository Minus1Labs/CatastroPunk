using UnityEngine;

public abstract class CardBehavior : MonoBehaviour
{
    public int itemStacks = 1;
    //No need to add a stack in ApplyUpgrade
    public abstract void ApplyUpgrade(Player player);
    public abstract void RemoveUpgrade(Player player);
    public abstract void AddStack(Player player);
    public virtual void OnProjectileHit(Player player) { }
    public virtual void OnEnemyKill(Player player) { }
    public virtual void OnProjectileMiss(Player player) { }
    public virtual void OnDamaged(Player player) { }
}