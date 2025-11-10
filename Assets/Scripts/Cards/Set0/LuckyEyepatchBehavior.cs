using UnityEngine;

public class LuckyEyepatchBehavior : CardBehavior
{
    public float luckPerStack = 0.3f;
    public float cashMultPerStack = 0.15f;

    public override void AddStack(Player player)
    {
        itemStacks++;
        player.additiveStats.luck += luckPerStack;
        player.additiveStats.cashMultiplier += cashMultPerStack;
    }

    public override void ApplyUpgrade(Player player)
    {
        AddStack(player);
        itemStacks--;
        
    }

    public override void RemoveUpgrade(Player player)
    {
        player.additiveStats.luck -= luckPerStack * itemStacks;
        player.additiveStats.cashMultiplier -= cashMultPerStack * itemStacks;
    }
}