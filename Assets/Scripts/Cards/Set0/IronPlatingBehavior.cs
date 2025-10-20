using UnityEngine;

public class IronPlatingBehavior : CardBehavior
{
    public int addMaxHealth = 2;
    public int healthPerStack = 2;
    public override void ApplyUpgrade(Player player)
    {
        player.additiveStats.health += addMaxHealth + healthPerStack;
        player.AddHealth(2);
    }

    public override void RemoveUpgrade(Player player)
    {
        player.additiveStats.health -= addMaxHealth + healthPerStack * itemStacks;
    }

    public override void AddStack(Player player)
    {
        player.additiveStats.health += healthPerStack;
        itemStacks++;
    }
}