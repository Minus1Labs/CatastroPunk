
public class CalculateDamageEvent : GenericEvent
{
    public float damage { get; set; }
    public CalculateDamageEvent(float PreDamage)
    {
        damage = PreDamage;
    }
}