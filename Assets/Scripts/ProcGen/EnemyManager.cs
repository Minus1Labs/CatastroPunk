using System.Collections.Generic;
using Enemies;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> enemy_prefabs = new List<GameObject>();


    //This will handle spawning enemies in rooms
    private Player player;
    private List<Enemy> alive_enemies = new List<Enemy>();


    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    /*
    public void SpawnEnemy(Vector3 worldPos, float difficultyMultiplier)
    {

    }*/

    public void AddEnemy(Enemy e)
    {
        alive_enemies.Add(e);
        e.characterEvents.OnTakeDamage += OnEnemyDamaged;
        e.characterEvents.OnKillEnemy += OnEnemyKilled;
    }

    public void OnEnemyDamaged(CharacterDamagedEvent cde)
    {
        player.characterEvents.RaiseTakeDamage(cde);
    }

    public void OnEnemyKilled(EnemyDiedEvent ede)
    {
        alive_enemies.Remove(ede.Enemy);
        EnemyDiedEvent edeEdit = new EnemyDiedEvent(ede.Enemy, player.GameObject(), ede.DeathPosition, ede.DeathCause);
        player.characterEvents.RaiseKill(edeEdit);
        ede.Enemy.characterEvents.OnTakeDamage -= OnEnemyDamaged;
        ede.Enemy.characterEvents.OnKillEnemy -= OnEnemyKilled;
    }
    
}