using System;
using UnityEngine;

public class CharacterEvents
{
    //public event Action<HitData> OnHitEnemy;
    public event Action<EnemyDiedEvent> OnKillEnemy;
    public event Action<CharacterDamagedEvent> OnTakeDamage;
    public event Action<ProjectileCreatedEvent> OnProjectileCreate;
    public event Action<CardAddedEvent> OnCardAdded;
    public event Action<CalculateDamageEvent> OnDamageCalculate;

    // Methods to raise events

    /*public void RaiseHit(HitData hit)
    {
        OnHitEnemy?.Invoke(hit);
    }*/

    public void RaiseKill(EnemyDiedEvent ede)
    {
        OnKillEnemy?.Invoke(ede);
    }

    public void RaiseProjectileCreate(ProjectileCreatedEvent pce)
    {
        OnProjectileCreate?.Invoke(pce);
    }

    public void RaiseCardAddedEvent(CardAddedEvent ce)
    {
        OnCardAdded?.Invoke(ce);
    }

    public void RaiseCalculateDamageEvent(CalculateDamageEvent cde)
    {
        OnDamageCalculate?.Invoke(cde);
    }

    public void RaiseTakeDamage(CharacterDamagedEvent cde)
    {
        OnTakeDamage?.Invoke(cde);
    }

    //public void RaiseHeal(float amount){OnHeal?.Invoke(amount);}
}