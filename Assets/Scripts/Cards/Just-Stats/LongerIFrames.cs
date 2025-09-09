using Unity.VisualScripting;
using UnityEngine;

public class LongerIFrames : Card
{
    void Awake() {
        cardName = "Longer I-Frames";
    }
    public override void ApplyUpgrade(Player player)
    {
        player.SetIFrameDur(2f, 0, -1f);
    }
}