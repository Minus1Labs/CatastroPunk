using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ProcGen
{
    public class RoomGenerator : MonoBehaviour
    {
        [SerializeField]
        private ScriptUtilities.FLOOR_NAMES floor_type =
            ScriptUtilities.FLOOR_NAMES.LAB;

        [SerializeField] private AssetLibrary asset_library;
        [SerializeField] private int room_y = 10;
        [SerializeField] private int room_x = 10;

        [SerializeField] Tilemap floorTilemap;

        private bool prefabsAreValid = false;

        private List<RoomLayouts.Room> roomsList;

        private Grid tilemapGrid;

        public void Start()
        {
            Debug.Log("Checking Prefabs");
            prefabsAreValid = asset_library.CheckAllPrefabs();
            Debug.Log("Checked prefabs");

            roomsList = RoomLayouts.GetRoomsFromDisk();
            Debug.Log("Got rooms from disk");

            if (floorTilemap == null)
            {
                Debug.LogError("Tilemap not set! Not continuing");
                return;
            }
            tilemapGrid = floorTilemap.layoutGrid;

            GenerateRoom(transform.position, roomsList[0]);
        }

        public void GenerateRoom(Vector2 roomOrigin, RoomLayouts.Room room)
        {
            //int index = (int)UnityEngine.Random.Range(0, roomsList.Count);
            PlaceTiles(roomOrigin);
            PlaceProps(roomOrigin, room);
        }

        //Later we can do patterns, but for now just set all tiles as the first
        public void PlaceTiles(Vector2 roomOrigin)
        {
            for (int i = 0; i < room_x; i++)
            {
                for (int j = 0; j < room_y; j++)
                {
                    floorTilemap.SetTile(new
                        Vector3Int(i + (int)roomOrigin.x, ((int)roomOrigin.y - j) - 1, 0),
                        asset_library.floor_tiles[0]);
                }
            }
        
        }

        public void PlaceProps(Vector2 roomOrigin, RoomLayouts.Room room)
        {
            PlacePrefabs(roomOrigin, room.Props._1x1, asset_library.prop_collider_1x1);
            PlacePrefabs(roomOrigin, room.Props._1x2, asset_library.prop_collider_1x2);
            PlacePrefabs(roomOrigin, room.Props._1x3, asset_library.prop_collider_1x3);
            PlacePrefabs(roomOrigin, room.Props._2x1, asset_library.prop_collider_2x1);
            PlacePrefabs(roomOrigin, room.Props._3x1, asset_library.prop_collider_3x1);
        }

        private void PlacePrefabs(Vector2 roomOrigin, List<RoomLayouts.Position> positions, GameObject[] assets)
        {
            if (assets.Length == 0)
            {
                Debug.Log("No Appropriate prefabs in the Asset Library");
            }
            foreach (RoomLayouts.Position pos in positions)
            {
                Vector2 world = ScriptUtilities.IsoGridToWorld(new Vector2(pos.x, pos.y));
                int randomAssetIndex = UnityEngine.Random.Range(0, assets.Length);
                Instantiate(assets [randomAssetIndex],
                    roomOrigin + world + new Vector2(assets[randomAssetIndex].transform.position.x, assets[randomAssetIndex].transform.position.y),
                    Quaternion.identity);
            }
        }

    }
}