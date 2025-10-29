using UnityEngine;

[System.Serializable]
public class PlayerStats : ScriptableObject
{
    [Header("Core Stats")]
    public int health = 18;
    public float damage = 40f;
    public float shotsPerSecond = 1.5f;
    public float moveSpeed = 1.5f;

    [Header("Dash")]
    public float dashDistance = 2f;
    public float dashCooldown = 4f;

    [Header("Energy System")]
    public float batteryCapacity = 10f;
    public float energyRegain = 2f;
    public float energyPerShot = 2f;

    [Header("Economy & Luck")]
    public float cashMultiplier = 1f;
    public float luck = 1f;

    // Optional: Copy constructor for pass-by-value semantics
    public PlayerStats(PlayerStats other)
    {
        health = other.health;
        damage = other.damage;
        shotsPerSecond = other.shotsPerSecond;
        moveSpeed = other.moveSpeed;
        dashDistance = other.dashDistance;
        dashCooldown = other.dashCooldown;
        batteryCapacity = other.batteryCapacity;
        energyRegain = other.energyRegain;
        energyPerShot = other.energyPerShot;
        cashMultiplier = other.cashMultiplier;
        luck = other.luck;
    }
    public PlayerStats() { }

    public void SetAllNum(float number)
    {
        health = (int)number;
        damage = number;
        shotsPerSecond = number;
        moveSpeed = number;
        dashDistance = number;
        dashCooldown = number;
        batteryCapacity = number;
        energyRegain = number;
        energyPerShot = number;
        cashMultiplier = number;
        luck = number;
    }

    // Optional: Clone method (explicit copy)
    public PlayerStats Clone() => new PlayerStats(this);
}