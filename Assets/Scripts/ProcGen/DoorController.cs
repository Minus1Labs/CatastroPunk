using System.Collections.Generic;
using UnityEngine;

public class DoorController : ISpecialInteractible
{
    private List<ISpecialInteractible> members;
    private List<int> requiredEnemies;
    private int puzzles;
    public DoorController()
    {
        members = new List<ISpecialInteractible>();
        requiredEnemies = new List<int>();
    }

    public void OpenIfCompleted()
    {
        if (requiredEnemies.Count == 0 && puzzles == 0)
        {
            SpecialInteract(ISpecialInteractible.SpecialInteractType.PUZZLE_ONLY);
        }
    }

    public void SetLastDoorTP(Vector2 pos)
    {
        if (members.Count < 1) { return; }
        
        LockedDoor ld = members[0] as LockedDoor; //dangerous
        Transform doorParent = ld.transform.parent;
        PlayerTeleporter ptp = doorParent.GetComponentInChildren<PlayerTeleporter>() ?? doorParent.GetComponent<PlayerTeleporter>();
        ptp.targetLocation = pos;
    }

    public void AddDoor(ISpecialInteractible si)
    {
        members.Add(si);
    }

    public void AddRequiredEnemy(Enemies.Enemy enemy)
    {
        if (requiredEnemies.Count == 0)
        {
            EventDispatcher.OnEnemyDied += OnEnemyDied;
        }
        requiredEnemies.Add(enemy.gameObject.GetInstanceID());
    }

    public void AddRequiredEnemy(int id)
    {
        if (requiredEnemies.Count == 0)
        {
            EventDispatcher.OnEnemyDied += OnEnemyDied;
        }
        requiredEnemies.Add(id);
    }

    //For now, let's only count the number of puzzles added and the number of puzzles completed
    public void AddPuzzle(int count)
    {
        puzzles += count;
    }

    private void OnEnemyDied(EnemyDiedEvent e)
    {
        requiredEnemies.Remove(e.Enemy.GetInstanceID());
        if (requiredEnemies.Count == 0)
        {
            SpecialInteract(ISpecialInteractible.SpecialInteractType.PUZZLE_ONLY);
            EventDispatcher.OnEnemyDied -= OnEnemyDied;
        }
    }

    public void SpecialInteract(ISpecialInteractible.SpecialInteractType it)
    {
        puzzles--;
        if (puzzles <= 0)
        {
            foreach (ISpecialInteractible si in members)
            {
                si.SpecialInteract(it);
            }
        }
    }

}