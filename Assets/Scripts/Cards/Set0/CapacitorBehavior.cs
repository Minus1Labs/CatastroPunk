using UnityEngine;

public class CapacitorBehavior : CardBehavior
{
    private float beforeUpgradeEPS = 0;
    private float totalDamageMult = 1;
    public float damageMultPerStack = 1.2f;
    private Player p;
    private bool isActive = true;

    public override void AddStack(Player player)
    {
        totalDamageMult += damageMultPerStack;
        itemStacks++;
    }

    public override void ApplyUpgrade(Player player)
    {
        p = player;
        p.characterEvents.OnDamageCalculate += GetCapacitorDamage;
        totalDamageMult *= player.GetBatteryCapacity() / player.GetShotEnergy();
        beforeUpgradeEPS = player.GetShotEnergy();
    }


    public override void RemoveUpgrade(Player player)
    {
        isActive = false;
        p.characterEvents.OnDamageCalculate -= GetCapacitorDamage;

        player.additiveStats.energyPerShot = beforeUpgradeEPS - player.baseStats.energyPerShot;
    }

    public void FixedUpdate()
    {
        if (p != null && isActive)
        {
            p.additiveStats.energyPerShot = p.currentCharge - p.baseStats.energyPerShot;
        }
    }

    public void GetCapacitorDamage(CalculateDamageEvent cde)
    {
        cde.damage *= (p.GetShotEnergy() / p.GetBatteryCapacity()) * totalDamageMult;
    }
}