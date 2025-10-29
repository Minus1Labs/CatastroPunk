using UnityEngine;
using System;

public static class EventDispatcher
{
    public static event Action<EnemyDiedEvent> OnEnemyDied;

    public static void RaiseEnemyDied(EnemyDiedEvent e)
    {
        OnEnemyDied?.Invoke(e);
    }
}