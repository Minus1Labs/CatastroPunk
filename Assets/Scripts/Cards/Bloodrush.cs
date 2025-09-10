using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Bloodrush", menuName = "Cards/Bloodrush")]
public class Bloodrush : Card
{
    void Awake()
    {
        cardName = "Bloodrush";
    }

    private float falloffTime = 10f;
    private int maxStacks = 6;
    private int currentStacks = 1;

    private float speedMult = 0.5f;
    private float damageMult = 2f;

    private int kills = 0;

    private Player p;

    public override void ApplyUpgrade(Player player)
    {
        p = player;
        player.SetSpeed(0.8f);
        player.SetDamage(0.5f);
    }

    public override void OnEnemyKill(Player player)
    {
        p = player;
        kills += 1;
        int tempStacks = currentStacks;
        if (currentStacks < maxStacks)
        {
            currentStacks++;
        }
        player.SetSpeed((currentStacks / tempStacks) * speedMult);
        player.SetDamage((currentStacks / tempStacks) * damageMult);
    }

    IEnumerator StackTimer(int killsAtTime)
    {
        yield return new WaitForSeconds(falloffTime);
        if (kills == killsAtTime)
        {
            ResetStacks();
        }
    }

    private void ResetStacks()
    {
        int tempStacks = currentStacks;
        currentStacks = 1;
        p.SetSpeed((currentStacks / tempStacks) * speedMult);
        p.SetDamage((currentStacks / tempStacks) * damageMult);
    }
}