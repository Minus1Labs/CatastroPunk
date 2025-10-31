using System;
using UnityEngine;

public class VampireTeethBehavior : CardBehavior
{
    private Player playerRef;
    public readonly float CHANCE_MULTIPLIER = 0.05f;
    public readonly float LUCK_EXPONENT = 2f;

    public override void AddStack(Player player)
    {
        itemStacks++;
    }

    public override void ApplyUpgrade(Player player)
    {
        playerRef = player;
        EventDispatcher.OnEnemyDied += OnEnemyKill;
    }

    public override void RemoveUpgrade(Player player)
    {
        EventDispatcher.OnEnemyDied -= OnEnemyKill;
    }

    public override void OnEnemyKill(EnemyDiedEvent e)
    {
        if (playerRef == null)
        {
            Debug.LogError("CardBehavior not set up correctly. Player reference is null!");
            return;
        }
        float luckPow = UnityEngine.Mathf.Pow(playerRef.GetLuck(), LUCK_EXPONENT);
        luckPow = luckPow < 0 ? 0 : luckPow;
        float successThreshold = CHANCE_MULTIPLIER * luckPow;
        if (UnityEngine.Random.Range(0f, 1f) <= successThreshold)
        {
            playerRef.AddHealth(1);
        }
    }
}