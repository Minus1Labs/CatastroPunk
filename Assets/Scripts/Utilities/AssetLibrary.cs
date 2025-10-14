using UnityEngine;
using UnityEngine.Tilemaps;


public class AssetLibrary : MonoBehaviour
{
    [Header("Reference the appropriate prefabs")]

    public GameObject[] prop_collider_1x1 = new GameObject[1];
    public GameObject[] prop_collider_1x2 = new GameObject[1];
    public GameObject[] prop_collider_1x3 = new GameObject[1];
    public GameObject[] prop_collider_2x2 = new GameObject[1];
    public GameObject[] prop_collider_2x1 = new GameObject[1];
    public GameObject[] prop_collider_3x1 = new GameObject[1];
    public GameObject[] wall_4x1 = new GameObject[1];
    public Tile[] floor_tiles = new Tile[1];
    public GameObject[] door_teleporter = new GameObject[1];
    public GameObject[] enemies = new GameObject[1];

    public bool CheckAllPrefabs()
    {
        return CheckProps() && CheckWalls() && CheckEnemies() && CheckDoors() && CheckFloors();
    }

    public bool CheckProps()
    {
        return CheckArray(prop_collider_1x1) &&
            CheckArray(prop_collider_1x2) &&
            CheckArray(prop_collider_1x3) &&
            CheckArray(prop_collider_2x2) &&
            CheckArray(prop_collider_2x1);
    }

    public bool CheckWalls()
    {
        return CheckArray(wall_4x1);
    }

    public bool CheckDoors()
    {
        return CheckArray(door_teleporter);
    }

    public bool CheckEnemies()
    {
        return CheckArray(enemies);
    }

    public bool CheckFloors()
    {
        for (int i = 0; i < floor_tiles.Length; i++)
        {
            if (floor_tiles[i] == null)
            {
                Debug.Log("Error with AssetLibrary " + name + " in " + floor_tiles.ToString());
                return false;
            }
        }
        return true;
    }

    private bool CheckArray(GameObject[] prefabs)
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            if (prefabs[i] == null)
            {
                Debug.Log("Error with AssetLibrary " + name + " in " + prefabs.ToString());
                return false;
            }
        }
        return true;
    }

}