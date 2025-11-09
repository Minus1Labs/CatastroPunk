using Enemies;
using UnityEngine;

public class CharacterDamagedEvent : GenericEvent
{
    public bool damagedIsPlayer;
    public bool damagedIsEnemy;
    public ScriptUtilities.DAMAGE_TYPE damage_type;
    public float damage;
    public Player player;
    public Enemy enemy;

    public CharacterDamagedEvent(Player p, Enemy e, float damage_dealt, bool damageSourceIsPlayer, ScriptUtilities.DAMAGE_TYPE DAMAGE_TYPE)
    {
        if (damageSourceIsPlayer)
        {
            damagedIsEnemy = true;
            damagedIsPlayer = false;
        }

        player = p;
        enemy = e;
        damage = damage_dealt;
        damage_type = DAMAGE_TYPE;
    }
}