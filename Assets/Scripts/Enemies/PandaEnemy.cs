using UnityEngine;

public class PandaEnemy : Enemies.Enemy
{
    [SerializeField] private float attackDuration = 1.5f;
    [SerializeField] private float attackWindupTime = 1.5f;
    [SerializeField] private float attackCooldownTime = 2.5f;
    [SerializeField] private float attackSweepConeDegrees = 22.5f;


    //Reverse stuff
    [SerializeField] private float reverseLookbackSeconds = 3f;
    [SerializeField] private float historyKeepSeconds = 2f;
    private struct PosSample
    {
        public float t;
        public Vector2 p;
        public PosSample(float t, Vector2 p) { this.t = t; this.p = p; }
    }
    private readonly System.Collections.Generic.LinkedList<PosSample> posHistory = new System.Collections.Generic.LinkedList<PosSample>();

    public override void Attack(GameObject target)
    {
        throw new System.NotImplementedException();
    }

    public override void Move()
    {
        throw new System.NotImplementedException();
    }

    public override void Reverse(int strength)
    {
        throw new System.NotImplementedException();
    }
}