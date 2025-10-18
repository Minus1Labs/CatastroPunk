public interface IDamageable
{
    //For vertical slice this will just take an int. 
    //Post v-slice I would like callers of this method to pass
    //themselves as an argument so we can handle further interactivity
    public void TakeDamage(float damage);
}