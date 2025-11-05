using System;
using UnityEngine;

public class CharacterEvents
{
    //public event Action<HitData> OnHitEnemy;
    public event Action<Enemies.Enemy> OnKillEnemy;

    //public event Action<DamageData> OnTakeDamage;

    public event Action<float> OnHeal;

    public event Action<ProjectileCreatedEvent> OnProjectileCreate;
    public event Action<CardAddedEvent> OnCardAdded;
    public event Action<CalculateDamageEvent> OnDamageCalculate;

    // Methods to raise events

    /*public void RaiseHit(HitData hit)
    {
        OnHitEnemy?.Invoke(hit);
    }*/

    public void RaiseKill(Enemies.Enemy enemy)
    {
        OnKillEnemy?.Invoke(enemy);
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

    //public void RaiseTakeDamage(DamageData damage){ OnTakeDamage?.Invoke(damage);}

    //public void RaiseHeal(float amount){OnHeal?.Invoke(amount);}
}