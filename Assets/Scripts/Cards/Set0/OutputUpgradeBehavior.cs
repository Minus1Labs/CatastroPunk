using UnityEngine;

public class OutputUpgradeBehavior : CardBehavior
{
    public int damagePerStack = 1;
    public override void AddStack(Player player)
    {
        player.additiveStats.damage += damagePerStack;
        itemStacks++;
    }

    public override void ApplyUpgrade(Player player)
    {
        AddStack(player);
        itemStacks--;
    }

    public override void RemoveUpgrade(Player player)
    {
        player.additiveStats.damage -= damagePerStack * itemStacks;
    }
}