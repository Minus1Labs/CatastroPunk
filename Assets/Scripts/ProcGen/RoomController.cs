using System.Collections.Generic;
using ProcGen;
using UnityEngine;


public class RoomController : MonoBehaviour
{
    // Keep track of everything
    public FloorGenerator.PathRoom pathroom;
    public List<GameObject> colliderProps;
    public List<GameObject> enemies;
    public DoorController doorController;
    public List<GameObject> puzzleObjects;
    public List<GameObject> vendingMachines;
    public bool roomIsCleared = true;

    // For detecting if player is in room.
    [SerializeField] private Rigidbody2D rbCollisionDetection;
    [SerializeField] private Collider2D playerDetectionCollider;
    public bool playerIsInRoom = false;

    // doors
    public GameObject doorLeft; //0
    public GameObject doorUp; //1
    public GameObject doorRight; //2
    public GameObject doorDown; //3
    

    // Rooms directly connected to this
    public RoomController Up;
    public RoomController Right;
    public RoomController Down;
    public RoomController Left;


    //Call this when all necessary vars are set
    public void Init()
    {
        if (enemies.Count > 0)
        {
            roomIsCleared = false;
            EventDispatcher.OnEnemyDied += OnEnemyDied;
        }

        if (puzzleObjects.Count > 0)
        {
            roomIsCleared = false;
        }

        if (roomIsCleared)
        {
            doorController.SpecialInteract(ISpecialInteractible.SpecialInteractType.PUZZLE_ONLY);
        }
    }

    private void OnEnemyDied(EnemyDiedEvent ede)
    {
        
        enemies.Remove(ede.Enemy.gameObject);
        if (enemies.Count == 0 && puzzleObjects.Count == 0 && !roomIsCleared)
        {
            roomIsCleared = true;
            doorController.SpecialInteract(ISpecialInteractible.SpecialInteractType.PUZZLE_ONLY);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInRoom = true;
            if (!roomIsCleared)
            {
                //TODO: Freeze enemies for a second, close doors, and play animations of enemies appearing
            }
        }
    }
    
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player"))
        {
            playerIsInRoom = false;
        }
    }
}