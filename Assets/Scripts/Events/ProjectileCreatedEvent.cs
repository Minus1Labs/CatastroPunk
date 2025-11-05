using UnityEngine;

public class ProjectileCreatedEvent : GenericEvent
{
    public GameObject ProjectileGO { get; private set; }
    public GameObject Owner { get; private set; }
    public Projectile projectile { get; private set; }

    public ProjectileCreatedEvent(GameObject projectileObject, GameObject projectileOwner)
    {
        ProjectileGO = projectileObject;
        Owner = projectileOwner;
        projectile = projectileObject.GetComponent<Projectile>() ?? projectileObject.GetComponentInChildren<Projectile>();
        EventMessage = Owner.name + " created projectile " + projectileObject.name;
    }
}