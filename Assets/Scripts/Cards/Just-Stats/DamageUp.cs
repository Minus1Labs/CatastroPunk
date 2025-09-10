using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageUp", menuName = "Cards/Just-Stats/DamageUp")]
public class DamageUp : Card
{
    void Awake()
    {
        cardName = "Damage Up - Multiplicative";
    }
    public override void ApplyUpgrade(Player player)
    {
        player.SetDamage(1.2f);
    }
}