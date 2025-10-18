using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Rendering;
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
        [SerializeField] private int room_count = 3;
        [SerializeField] private int room_offset = 250;
        [SerializeField] Tilemap floorTilemap;

        private bool prefabsAreValid = false;

        private List<RoomLayouts.Room> roomsList;

        private Grid tilemapGrid;

        public void Start()
        {
            Debug.Log("Checking Prefabs");
            //prefabsAreValid = asset_library.CheckAllPrefabs();
            Debug.Log("Checked prefabs");

            roomsList = RoomLayouts.GetRoomsFromDisk();
            Debug.Log("Got rooms from disk");

            if (floorTilemap == null)
            {
                Debug.LogError("Tilemap not set! Not continuing");
                return;
            }
            tilemapGrid = floorTilemap.layoutGrid;

            Vector3 roompos = transform.position;
            for (int i = 0; i < room_count; i++)
            {
                GenerateRoom(roompos, roomsList[UnityEngine.Random.Range(0, roomsList.Count)]);
                roompos.x += room_offset;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Vector3 room_origin = transform.position;
            for (int i = 0; i < room_count; i++)
            {
                Vector3 topCorner = room_origin + WorldFromIsoV3(0, room_y);
                Vector3 farCorner = room_origin + WorldFromIsoV3(room_x, room_y);
                Vector3 bottomCorner = room_origin + WorldFromIsoV3(room_x, 0);
                Gizmos.DrawLine(room_origin, topCorner);
                Gizmos.DrawLine(room_origin, bottomCorner);
                Gizmos.DrawLine(farCorner, topCorner);
                Gizmos.DrawLine(farCorner, bottomCorner);
                room_origin.x += room_offset;
            }

        }

        private Vector3 WorldFromIsoV3(float x, float y)
        {
            return ScriptUtilities.IsoGridToWorld(new Vector2(x, y));
        }

        public void GenerateRoom(Vector2 roomOrigin, RoomLayouts.Room room)
        {
            //int index = (int)UnityEngine.Random.Range(0, roomsList.Count);
            PlaceTiles(roomOrigin);
            PlaceProps(roomOrigin, room);
            PlaceDoorTeleporter(roomOrigin, room);
        }

        //Later we can do patterns, but for now just set all tiles as the first
        public void PlaceTiles(Vector2 roomOrigin)
        {
            Vector2 tileRoomOrigin = ScriptUtilities.WorldToIsoGrid(roomOrigin);

            for (int i = 0; i < room_x; i++)
            {
                for (int j = 0; j < room_y; j++)
                {
                    floorTilemap.SetTile(new
                        Vector3Int(i + (int)tileRoomOrigin.x, (-(int)tileRoomOrigin.y - j) - 1, 0),
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

        private GameObject PlacePrefabs(Vector2 roomOrigin, List<RoomLayouts.Position> positions, GameObject[] assets)
        {
            if (assets.Length == 0)
            {
                Debug.Log("No Appropriate prefabs in the Asset Library");
                return null;
            }
            GameObject lastInstantiated = null;
            foreach (RoomLayouts.Position pos in positions)
            {
                Vector2 world = ScriptUtilities.IsoGridToWorld(new Vector2(pos.x, pos.y));
                int randomAssetIndex = UnityEngine.Random.Range(0, assets.Length);
                lastInstantiated = Instantiate(assets[randomAssetIndex],
                    roomOrigin + world + new Vector2(assets[randomAssetIndex].transform.position.x, assets[randomAssetIndex].transform.position.y),
                    Quaternion.identity);
            }
            return lastInstantiated;
        }

        private void PlaceDoorTeleporter(Vector2 roomOrigin, RoomLayouts.Room room)
        {
            List<RoomLayouts.Position> doorPos = new List<RoomLayouts.Position>();
            doorPos.Add(room.Doors.Entrance_pos);
            GameObject doorPrefab = PlacePrefabs(roomOrigin, doorPos, asset_library.door_teleporter);
            PlayerTeleporter ptp = doorPrefab.GetComponentInChildren<PlayerTeleporter>();
            
            if (ptp != null)
            {
                Vector2 location = roomOrigin;
                location.x += room_offset;
                ptp.targetLocation = location;
                Debug.Log("set ptp location to " + location);
            }

            floorTilemap.SetTile(new
                        Vector3Int(10 + (int)roomOrigin.x, ((int)roomOrigin.y - 4) - 1, 0),
                        asset_library.floor_tiles[0]);
            
            floorTilemap.SetTile(new
                        Vector3Int(10 + (int)roomOrigin.x, ((int)roomOrigin.y - 5) - 1, 0),
                        asset_library.floor_tiles[0]);
        }

    }
}