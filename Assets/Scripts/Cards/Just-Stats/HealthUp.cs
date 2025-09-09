using Unity.VisualScripting;
using UnityEngine;

public class HealthUp : Card
{
    void Awake() {
        cardName = "Health Up - Additive";
    }
    public override void ApplyUpgrade(Player player)
    {
        player.SetHealth(1f, 3, -1);
    }
}