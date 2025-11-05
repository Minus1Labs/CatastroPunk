using UnityEngine;

public class EmDashBehavior : CardBehavior
{
    public float dashDistancePerStack = 2f;
    public float dashCdPerStack = 2f;
    public override void AddStack(Player player)
    {
        player.multStats.dashDistance *= dashDistancePerStack;
        player.multStats.dashCooldown *= dashCdPerStack;
        itemStacks++;
    }

    public override void ApplyUpgrade(Player player)
    {
        AddStack(player);
        itemStacks--;
    }

    public override void RemoveUpgrade(Player player)
    {
        player.multStats.dashDistance /= dashDistancePerStack * itemStacks;
        player.multStats.dashCooldown /= dashCdPerStack * itemStacks;
    }
}