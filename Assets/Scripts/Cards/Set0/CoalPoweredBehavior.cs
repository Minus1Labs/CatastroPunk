using UnityEngine;

public class CoalPoweredBehavior : CardBehavior
{
    public float luckPerStack = -0.15f;
    public float cashMultPerStack = 0.3f;
    public override void AddStack(Player player)
    {
        player.additiveStats.luck += luckPerStack;
        player.multStats.cashMultiplier += cashMultPerStack;
        itemStacks++;
    }

    public override void ApplyUpgrade(Player player)
    {
        AddStack(player);
        itemStacks--;
    }

    public override void RemoveUpgrade(Player player)
    {
        player.additiveStats.luck -= luckPerStack * itemStacks;
        player.multStats.cashMultiplier -= cashMultPerStack * itemStacks;
    }
}