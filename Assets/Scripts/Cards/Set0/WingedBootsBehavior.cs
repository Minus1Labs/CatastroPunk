using UnityEngine;

public class WingedBootsBehavior : CardBehavior
{
    public float msPerStack = 0.3f;
    public override void AddStack(Player player)
    {
        itemStacks++;
        player.additiveStats.moveSpeed += msPerStack;
    }

    public override void ApplyUpgrade(Player player)
    {
        AddStack(player);
        itemStacks--;
    }

    public override void RemoveUpgrade(Player player)
    {
        player.additiveStats.moveSpeed -= itemStacks * msPerStack;
    }
}