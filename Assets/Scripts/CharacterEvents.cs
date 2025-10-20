using System;
using UnityEngine;

public class CharacterEvents : MonoBehaviour
{
    // Called when this character hits an enemy.
    //public event Action<HitData> OnHitEnemy;

    // Called when this character kills an enemy.
    public event Action<Enemies.Enemy> OnKillEnemy;

    // Called when this character takes damage.
    //public event Action<DamageData> OnTakeDamage;

    // Called when this character heals.
    public event Action<float> OnHeal;

    // Methods to raise events

    /*public void RaiseHit(HitData hit)
    {
        OnHitEnemy?.Invoke(hit);
    }*/

    public void RaiseKill(Enemies.Enemy enemy)
    {
        OnKillEnemy?.Invoke(enemy);
    }

   //public void RaiseTakeDamage(DamageData damage){ OnTakeDamage?.Invoke(damage);}

    //public void RaiseHeal(float amount){OnHeal?.Invoke(amount);}
}