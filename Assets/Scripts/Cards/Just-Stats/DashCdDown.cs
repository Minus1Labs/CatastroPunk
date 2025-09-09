using Unity.VisualScripting;
using UnityEngine;

public class DashCdDown : Card
{
    void Awake() {
        cardName = "Lower Dash Cooldown - Multiplicative";
    }
    public override void ApplyUpgrade(Player player)
    {
        player.SetDashCd(0.5f, 0, -1f);
    }
}