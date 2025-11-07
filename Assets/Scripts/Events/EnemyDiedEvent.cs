using UnityEngine;

public class EnemyDiedEvent : GenericEvent
{
    public Enemies.Enemy Enemy { get; private set; }
    public GameObject Killer { get; private set; }
    public Vector3 DeathPosition { get; private set; }
    public string DeathCause { get; private set; }

    public EnemyDiedEvent(Enemies.Enemy enemy, GameObject killer = null, Vector3? position = null, string cause = null)
    {
        Enemy = enemy;
        Killer = killer;
        DeathPosition = position ?? enemy.transform.position;
        DeathCause = cause ?? "Unknown";
    }
}