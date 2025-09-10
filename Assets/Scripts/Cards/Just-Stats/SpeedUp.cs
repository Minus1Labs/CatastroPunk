using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeedUp", menuName = "Cards/Just-Stats/SpeedUp")]
public class SpeedUp : Card
{
    void Awake()
    {
        cardName = "Speed Up - Multiplicative";
    }
    public override void ApplyUpgrade(Player player)
    {
        player.SetSpeed(1.2f);
    }
}