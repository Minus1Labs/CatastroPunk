using UnityEngine;

public class CapacitorBehavior : CardBehavior
{
    private float beforeUpgradeEPS = 0;
    private float totalDamageMult = 0;
    public float damageMultPerStack = 1.2f;

    public override void AddStack(Player player)
    {
        player.multStats.damage *= damageMultPerStack;
        SetEPStoCap(player);
        totalDamageMult += damageMultPerStack;
        itemStacks++;
    }

    public override void ApplyUpgrade(Player player)
    {
        beforeUpgradeEPS = player.GetShotEnergy();
        totalDamageMult = player.GetDamage();
        SetEPStoCap(player);

        player.multStats.damage *= damageMultPerStack;
        totalDamageMult *= damageMultPerStack;
        totalDamageMult = player.GetDamage() / totalDamageMult;
    }

    private void SetEPStoCap(Player p)
    {
        float currentShotEnergy = p.GetShotEnergy();
        float batteryCapacity = p.GetBatteryCapacity();

        p.additiveStats.energyPerShot = batteryCapacity - p.baseStats.energyPerShot;
        p.multStats.energyPerShot = 1f;

        float damageResult = batteryCapacity / currentShotEnergy;
        p.multStats.damage *= damageResult;
        totalDamageMult += damageMultPerStack;
    }

    public override void RemoveUpgrade(Player player)
    {
        player.additiveStats.energyPerShot = beforeUpgradeEPS - player.baseStats.energyPerShot;
        player.multStats.damage /= totalDamageMult;
    }
}