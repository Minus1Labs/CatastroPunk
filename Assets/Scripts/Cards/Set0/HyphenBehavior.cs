using UnityEngine;

public class HyphenBehavior : CardBehavior
{
    public float baseDashCdMult = 0.5f;
    public float baseDashDistMult = 0.5f;
    public float cdMultPerStack = 0.8f;
    public float distMultPerStack = 1f;
    public override void AddStack(Player player)
    {
        player.multStats.dashCooldown *= cdMultPerStack;
        player.multStats.dashDistance *= distMultPerStack;
        itemStacks++;
    }

    public override void ApplyUpgrade(Player player)
    {
        player.multStats.dashCooldown *= baseDashCdMult;
        player.multStats.dashDistance *= baseDashDistMult;
    }

    public override void RemoveUpgrade(Player player)
    {
        player.multStats.dashCooldown /= baseDashCdMult;
        player.multStats.dashDistance /= baseDashDistMult;

        int effectiveStacks = itemStacks - 1;
        if (effectiveStacks > 0)
        {
            player.multStats.dashCooldown /= cdMultPerStack * effectiveStacks;
            player.multStats.dashDistance /= distMultPerStack * effectiveStacks;
        }
    }
}