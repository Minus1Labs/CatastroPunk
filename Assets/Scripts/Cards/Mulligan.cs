using Unity.VisualScripting;
using UnityEngine;

public class Mulligan : Card
{
    void Awake()
    {
        cardName = "Mulligan";
    }
    public override void ApplyUpgrade(Player player) { }

    public override void OnProjectileMiss(Player player)
    {
        float rand = Random.Range(0f, 1f);
        if (rand < 0.8f) {
            player.SetCurrentAmmo(1f, 1, -1);
        }
    }
}