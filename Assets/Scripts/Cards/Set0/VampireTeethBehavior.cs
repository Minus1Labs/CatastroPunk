using UnityEngine;

public class VampireTeethBehavior : CardBehavior
{

    public override void AddStack(Player player)
    {
        throw new System.NotImplementedException();
    }

    public override void ApplyUpgrade(Player player)
    {
        throw new System.NotImplementedException();
    }

    public override void RemoveUpgrade(Player player)
    {
        throw new System.NotImplementedException();
    }

    public override void OnEnemyKill(Player player)
    {
        base.OnEnemyKill(player);
    }
}