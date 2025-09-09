using Unity.VisualScripting;
using UnityEngine;

public class DamageUp : Card
{
    void Awake() {
        cardName = "Damage Up - Multiplicative";
    }
    public override void ApplyUpgrade(Player player)
    {
        player.SetDamage(1.2f);
    }
}