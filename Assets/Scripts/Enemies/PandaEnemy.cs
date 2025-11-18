using UnityEngine;

public class PandaEnemy : Enemies.Enemy
{
    [Header("Panda-Specific Constants")]
    [SerializeField] private float attackDuration = 1f;
    [SerializeField] private float attackWindupTime = 1.5f;
    [SerializeField] private float attackCooldownTime = 2.5f;
    [SerializeField] private float attackSweepConeDegrees = 22.5f;
    [SerializeField] private int laserDamage = 1;
    [SerializeField] public int health = 12;

    [Header("Components")]
    [SerializeField] private Animator antAnimator;
    [SerializeField] private PandaProjectile projectilePrefab;



    //Reverse stuff
    [Header("Reversible Constants")]
    [SerializeField] private float reverseLookbackSeconds = 3f;
    [SerializeField] private float historyKeepSeconds = 2f;
    private struct PosSample
    {
        public float t;
        public Vector2 p;
        public PosSample(float t, Vector2 p) { this.t = t; this.p = p; }
    }
    private readonly System.Collections.Generic.LinkedList<PosSample> posHistory = new System.Collections.Generic.LinkedList<PosSample>();

    //panda AI state variables
    private bool isActive;
    private bool isAggroed;
    private bool isIdle;
    private bool isFiring;
    private Player player;

    public void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            //Unsafe cast
            player = p.GetComponent<Player>() ?? p.GetComponentInChildren<Player>();
        }
    }

    public void FixedUpdate()
    {

    }

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