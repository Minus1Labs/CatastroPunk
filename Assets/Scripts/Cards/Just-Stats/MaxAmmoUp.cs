using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "MaxAmmoUp", menuName = "Cards/Just-Stats/MaxAmmoUp")]
public class MaxAmmoUp : Card
{
    void Awake()
    {
        cardName = "Ammo Up - Additive";
    }
    public override void ApplyUpgrade(Player player)
    {
        player.SetAmmoPerRoom(1f, 3, -1);
        player.SetCurrentAmmo(1f, 3, -1);
    }
}