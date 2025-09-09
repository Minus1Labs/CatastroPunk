using Unity.VisualScripting;
using UnityEngine;

public class SpeedUp : Card
{
    void Awake() {
        cardName = "Speed Up - Multiplicative";
    }
    public override void ApplyUpgrade(Player player)
    {
        player.SetSpeed(1.2f);
    }
}