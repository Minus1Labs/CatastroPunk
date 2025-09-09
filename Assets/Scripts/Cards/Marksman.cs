using Unity.VisualScripting;
using UnityEngine;

public class Marksman : Card
{
    void Awake()
    {
        cardName = "Marksman";
    }

    private float maxBuff = 2.0f;
    private float currentBuff = 1.0f;

    private const float baseBuff = 1.0f;
    private float minBuff = 0.4f;
    private float stepAdditive = 0.2f;
    public override void ApplyUpgrade(Player player) { }

    public override void OnProjectileHit(Player player)
    {
        float tempDamage = currentBuff;
        if (currentBuff < maxBuff)
        {
            currentBuff += stepAdditive;
        }
        player.SetDamage(currentBuff / tempDamage);
    }

    public override void OnProjectileMiss(Player player)
    {
        float tempDamage = currentBuff;
        if (currentBuff > minBuff)
        {
            currentBuff -= stepAdditive;
        }
        player.SetDamage(currentBuff / tempDamage);
    }
}