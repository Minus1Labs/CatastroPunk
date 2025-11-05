using UnityEngine;

public class AABatteryBehavior : CardBehavior
{
    public float capPerStack = 4f;
    public override void AddStack(Player player)
    {
        player.additiveStats.batteryCapacity += capPerStack;
        itemStacks++;
    }

    public override void ApplyUpgrade(Player player)
    {
        AddStack(player);
        itemStacks--;
    }

    public override void RemoveUpgrade(Player player)
    {
        player.additiveStats.batteryCapacity -= capPerStack * itemStacks;
    }
}