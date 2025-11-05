using UnityEngine;
using System;

public static class EventDispatcher
{
    public static event Action<EnemyDiedEvent> OnEnemyDied;

    public static event Action<CardAddedEvent> OnCardAdded;

    public static void RaiseEnemyDied(EnemyDiedEvent e)
    {
        OnEnemyDied?.Invoke(e);
    }
    public static void RaiseCardAddedEvent(CardAddedEvent ce)
    {
        OnCardAdded?.Invoke(ce);
    }
}