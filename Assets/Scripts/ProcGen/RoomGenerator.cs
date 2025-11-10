using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace ProcGen
{
    public class RoomGenerator : MonoBehaviour
    {
        [SerializeField]
        private ScriptUtilities.FLOOR_NAMES floor_type =
            ScriptUtilities.FLOOR_NAMES.LAB;

        [SerializeField] private AssetLibrary asset_library;
        [SerializeField] Tilemap floorTilemap;
        [SerializeField] private GameObject RoomControllerPrefab;
        private List<FloorGenerator.PathRoom> roomPath;
        private Grid tilemapGrid;
        private bool setVarsFinished = false;
        private float roomSpacing = 50f;
        public RoomController[,] RoomControllerGrid = new RoomController[15,15];

        public void SetVars(AssetLibrary theme, List<FloorGenerator.PathRoom> rpath, Tilemap groundmap)
        {
            asset_library = theme;
            roomPath = rpath;
            floorTilemap = groundmap;

            setVarsFinished = true;
        }

        //TODO: Make prefab for start and end room that includes a roomcontroller.
        //Require those as necessary for this
        //Then, include those in linking and grid
        public void DoRoomGen()
        {
            if (!setVarsFinished)
            {
                Debug.LogError("Error: Vars were not set from FloorGenerator");
            }

            if (floorTilemap == null)
            {
                Debug.LogError("Tilemap not set! Not continuing");
                return;
            }

            tilemapGrid = floorTilemap.layoutGrid;

            Vector3 roompos = transform.position;
            FloorGenerator.PathRoom boss;

            RoomController rootRoom = null;

            for (int i = 0; i < roomPath.Count; i++)
            {

                FloorGenerator.PathRoom pr = roomPath[i];
                //Assume that the first room has its door set to the correct exit point
                if (pr.roomTile.ESCAPE_CONDITION == RoomTile.ROOM_ESCAPE_CONDITION.START)
                {
                    continue;
                }
                else if (pr.roomTile.ESCAPE_CONDITION == RoomTile.ROOM_ESCAPE_CONDITION.BOSS)
                {
                    boss = pr;
                    continue;
                }

                Vector2 rmPosUnscaled = new Vector2(pr.x, pr.y);
                roompos = ScriptUtilities.IsoGridToWorld(rmPosUnscaled * roomSpacing);

                GameObject rmControllerGO = Instantiate(RoomControllerPrefab, roompos + RoomControllerPrefab.transform.position, Quaternion.identity);
                RoomController rmController = rmControllerGO.GetComponent<RoomController>();
                if (rmController == null)
                {
                    Debug.LogError("RoomController prefab is not set up correctly!");
                    return;
                }

                rmController.pathroom = pr;
                RoomControllerGrid[pr.x, pr.y] = rmController;


                GenerateRoom(roompos, pr.roomTile.layout, rmPosUnscaled, rmController);

                if (rootRoom == null)
                {
                    rootRoom = rmController;
                }
                rmController.Init();
            }

            //Fill out graph of nodes now that they are all placed
            RecursiveSetAdjacent(rootRoom);


            //TODO DO LAST ROOM BEHAVIOR
            //TODO DO DOOR TO LAST ROOM TP

            //TODO: REPLACE TEMPORARY SOLUTION FOR BOSS ROOM AND START ROOM BACKTRACK TP
            RoomController beforeBoss = RoomControllerGrid[roomPath[roomPath.Count - 2].x, roomPath[roomPath.Count - 2].y];
            FloorGenerator.PathRoom bossPathRoom = roomPath[roomPath.Count - 1];
            int xdir = bossPathRoom.x - beforeBoss.pathroom.x;
            int ydir = bossPathRoom.y - beforeBoss.pathroom.y;

            if (xdir < 0 && beforeBoss.doorLeft != null)
            {
                PlayerTeleporter tp = beforeBoss.doorLeft.GetComponent<PlayerTeleporter>() ?? beforeBoss.doorLeft.GetComponentInChildren<PlayerTeleporter>();
                tp.targetLocation = new Vector2(92, -59);
            }
            else if (xdir > 0 && beforeBoss.doorRight != null)
            {
                PlayerTeleporter tp = beforeBoss.doorRight.GetComponent<PlayerTeleporter>() ?? beforeBoss.doorRight.GetComponentInChildren<PlayerTeleporter>();
                tp.targetLocation = new Vector2(92, -59);
            }
            else if (ydir > 0 && beforeBoss.doorUp != null)
            {
                PlayerTeleporter tp = beforeBoss.doorUp.GetComponent<PlayerTeleporter>() ?? beforeBoss.doorUp.GetComponentInChildren<PlayerTeleporter>();
                tp.targetLocation = new Vector2(92, -59);
            }
            else if (ydir < 0 && beforeBoss.doorDown != null)
            {
                PlayerTeleporter tp = beforeBoss.doorDown.GetComponent<PlayerTeleporter>() ?? beforeBoss.doorDown.GetComponentInChildren<PlayerTeleporter>();
                tp.targetLocation = new Vector2(92, -59);
            }

            //Connect start door
            PlayerTeleporter ptp = rootRoom.doorDown.GetComponent<PlayerTeleporter>() ?? rootRoom.doorDown.GetComponentInChildren<PlayerTeleporter>();
            ptp.targetLocation = new Vector2(30, 7);


        }

        private void RecursiveSetAdjacent(RoomController current)
        {
            if (current.Up == null && RoomControllerGrid[current.pathroom.x, current.pathroom.y + 1] != null)
            {
                RoomController up = RoomControllerGrid[current.pathroom.x, current.pathroom.y + 1];
                current.Up = up;
                up.Down = current;
                RecursiveSetAdjacent(up);
            }

            if (current.Right == null && RoomControllerGrid[current.pathroom.x + 1, current.pathroom.y] != null)
            {
                RoomController right = RoomControllerGrid[current.pathroom.x + 1, current.pathroom.y];
                current.Right = right;
                right.Left = current;
                RecursiveSetAdjacent(right);
            }

            if (current.Down == null && RoomControllerGrid[current.pathroom.x, current.pathroom.y - 1] != null)
            {
                RoomController down = RoomControllerGrid[current.pathroom.x, current.pathroom.y - 1];
                current.Down = down;
                down.Up = current;
                RecursiveSetAdjacent(down);
            }

            if (current.Left == null && RoomControllerGrid[current.pathroom.x - 1, current.pathroom.y] != null)
            {
                RoomController left = RoomControllerGrid[current.pathroom.x - 1, current.pathroom.y];
                current.Left = left;
                left.Right = current;
                RecursiveSetAdjacent(left);
            }
        }

        private Vector3 WorldFromIsoV3(float x, float y)
        {
            return ScriptUtilities.IsoGridToWorld(new Vector2(x, y));
        }

        public DoorController GenerateRoom(Vector2 roomOrigin, RoomLayouts.Room room,
            Vector2 roomGridPos, RoomController rmc)
        {
            //Assume that 0 doorslayout means no room layout for now
            if (room.DoorsLayout == 0)
            {
                Debug.LogError("No layout found for room at " + roomOrigin);
                return null;
            }



            DoorController dc = new DoorController();
            rmc.doorController = dc;

            PlaceTiles(roomOrigin, room);
            PlaceWalls(roomOrigin, room, rmc);
            PlaceProps(roomOrigin, room, rmc);
            PlaceDoors(roomOrigin, room, dc, roomGridPos, rmc);
            PlaceEnemies(roomOrigin, room, dc, rmc);

            //TODO ADD PUZZLES BACK IN
            //PlacePuzzles(roomOrigin, room.Puzzles, dc);

            foreach (GameObject enemyGO in rmc.enemies)
            {
                enemyGO.SetActive(true);
                dc.AddRequiredEnemy(enemyGO.GetInstanceID());
            }
            
            
            dc.OpenIfCompleted();
            return dc;
        }

        public void PlaceWalls(Vector2 roomOrigin, RoomLayouts.Room room, RoomController rmc)
        {
            //For now we are only considering 10x10 room shapes
            List<RoomLayouts.Position> ypos = new List<RoomLayouts.Position>();
            ypos.Add(new RoomLayouts.Position(new Vector2(0, 0)));
            ypos.Add(new RoomLayouts.Position(new Vector2(0, 5)));

            List<RoomLayouts.Position> xpos = new List<RoomLayouts.Position>();
            xpos.Add(new RoomLayouts.Position(new Vector2(0, 9)));
            xpos.Add(new RoomLayouts.Position(new Vector2(5, 9)));

            PlacePrefabs(roomOrigin, xpos, asset_library.walls_x, rmc, false, false);
            PlacePrefabs(roomOrigin, ypos, asset_library.walls_y, rmc, false, false);
        }

        //Later we can do patterns, but for now just set all tiles as the first
        public void PlaceTiles(Vector2 roomOrigin, RoomLayouts.Room room)
        {
            Vector2 tileRoomOrigin = ScriptUtilities.WorldToIsoGrid(roomOrigin);

            for (int i = 0; i < room.RoomSize.x; i++)
            {
                for (int j = 0; j < room.RoomSize.y; j++)
                {
                    floorTilemap.SetTile(new
                        Vector3Int((int)tileRoomOrigin.y + j, -(i + (int)tileRoomOrigin.x) - 1, 0),
                        asset_library.floor_tiles[0]);
                }
            }
        }

        public void PlaceProps(Vector2 roomOrigin, RoomLayouts.Room room, RoomController rmc)
        {
            PlacePrefabs(roomOrigin, room.Props._1x1, asset_library.prop_collider_1x1, rmc, true, false);
            PlacePrefabs(roomOrigin, room.Props._1x2, asset_library.prop_collider_1x2, rmc, true, false);
            PlacePrefabs(roomOrigin, room.Props._1x3, asset_library.prop_collider_1x3, rmc, true, false);
            PlacePrefabs(roomOrigin, room.Props._2x1, asset_library.prop_collider_2x1, rmc, true, false);
            PlacePrefabs(roomOrigin, room.Props._3x1, asset_library.prop_collider_3x1, rmc, true, false);
        }

        public void PlaceEnemies(Vector2 roomOrigin, RoomLayouts.Room room, DoorController dc, RoomController rmc)
        {
            if (room.EnemySpawns == null || room.EnemySpawns.Count == 0) { return; }
            PlacePrefabs(roomOrigin, room.EnemySpawns, asset_library.enemies, rmc,  false, true);
        }

        private void PlacePrefabs(Vector2 roomOrigin, List<RoomLayouts.Position> positions, GameObject[] assets, RoomController rmc, bool isProp, bool isEnemy)
        {
            if (positions == null)
            {
                return;
            }
            if (assets == null || assets.Length == 0)
            {
                Debug.Log("No Appropriate prefabs in the Asset Library");
                return;
            }
            foreach (RoomLayouts.Position pos in positions)
            {
                Vector2 world = ScriptUtilities.IsoGridToWorld(new Vector2(pos.x, pos.y));
                int randomAssetIndex = UnityEngine.Random.Range(0, assets.Length);
                GameObject assetPlaced = Instantiate(assets[randomAssetIndex],
                    roomOrigin + world + new Vector2(assets[randomAssetIndex].transform.position.x, assets[randomAssetIndex].transform.position.y),
                    Quaternion.identity);

                assetPlaced.transform.SetParent(rmc.transform);
                

                if (isProp)
                {
                    rmc.colliderProps.Add(assetPlaced);
                }

                if (isEnemy)
                {
                    rmc.enemies.Add(assetPlaced);
                }
            }
        }

        private void PlacePuzzles(Vector2 roomOrigin, List<RoomLayouts.Puzzle> puzzleInfo, DoorController dc)
        {
            if (puzzleInfo == null || puzzleInfo.Count == 0) { return; }
            foreach (RoomLayouts.Puzzle p in puzzleInfo)
            {
                if (p.Type.ToLower() == "wire")
                {
                    if (p.Positions.Count < 2 || p.Modifiers.Count < 1 || asset_library.wire_puzzle_start[0] == null || asset_library.wire_puzzle_end[0] == null)
                    {
                        Debug.Log("Error placing wire puzzle! One or more of the JSON inputs are invalid OR the assetlibrary wires are set up incorrectly");
                    }

                    PlaceWirePuzzle(roomOrigin, p.Positions[0], p.Positions[1], p.Modifiers[0],
                        asset_library.wire_puzzle_start[0], asset_library.wire_puzzle_end[0], dc);
                }
            }
        }

        private void PlaceWirePuzzle(Vector2 roomOrigin, RoomLayouts.Position start, RoomLayouts.Position end,
            int wireLength, GameObject wireStartPrefab, GameObject wireEndPrefab, DoorController dc)
        {
            if (wireStartPrefab == null || wireEndPrefab == null)
            {
                Debug.LogError("Tried to place a wire puzzle, but prefabs were null");
            }

            Vector2 wireStartWorld = ScriptUtilities.IsoGridToWorld(new Vector2(start.x, start.y)) +
                roomOrigin +
                new Vector2(wireStartPrefab.transform.position.x, wireStartPrefab.transform.position.y);
            GameObject wsGameObject = Instantiate(wireStartPrefab, wireStartWorld, Quaternion.identity);
            WireSource ws = wsGameObject.GetComponentInChildren<WireSource>();
            if (ws == null)
            {
                Debug.LogError("WireSource prefab doesn't contain a WireSource script");
            }
            ws.wireLength = wireLength;

            Vector2 wireEndWorld = ScriptUtilities.IsoGridToWorld(new Vector2(end.x, end.y)) +
                roomOrigin +
                new Vector2(wireEndPrefab.transform.position.x, wireEndPrefab.transform.position.y);
            GameObject wrGameObject = Instantiate(wireEndPrefab, wireEndWorld, Quaternion.identity);
            WireReceiver wr = wrGameObject.GetComponent<WireReceiver>() ?? wrGameObject.GetComponentInChildren<WireReceiver>();
            wr.interactible = dc;
            dc.AddPuzzle(1);

            if (wr == null)
            {
                Debug.LogError("WireReceiver prefab doesn't contain a WireReceiver script");
            }

        }

        private void PlaceDoors(Vector2 roomOrigin, RoomLayouts.Room room, DoorController dc, Vector2 roomGridPos, RoomController rmc)
        {
            if (room.DoorsLayout != 0)
            {
                bool tl = false; bool tr = false; bool br = false; bool bl = false;
                int tempLayoutNum = room.DoorsLayout;
                if (tempLayoutNum % 2 == 1)
                {
                    tl = true;
                }
                tempLayoutNum >>= 1;
                if (tempLayoutNum % 2 == 1)
                {
                    tr = true;
                }
                tempLayoutNum >>= 1;
                if (tempLayoutNum % 2 == 1)
                {
                    br = true;
                }
                tempLayoutNum >>= 1;
                if (tempLayoutNum % 2 == 1)
                {
                    bl = true;
                }
                //TODO: temp solution
                Vector2 tlPos = new Vector2(0, 4);
                Vector2 trPos = new Vector2(4, 9);
                Vector2 brPos = new Vector2(9, 4);
                Vector2 blPos = new Vector2(4, 0);

                PlayerTeleporter tp = null;

                //scuffed
                if (tl)
                {
                    GameObject doorPrefab = PlaceDoorTeleporter(roomOrigin, new RoomLayouts.Position(tlPos), asset_library.door_teleporter[0]);
                    LockedDoor doorInteractible = doorPrefab.GetComponent<LockedDoor>() ?? doorPrefab.GetComponentInChildren<LockedDoor>();
                    tp = doorPrefab.GetComponent<PlayerTeleporter>() ?? doorPrefab.GetComponentInChildren<PlayerTeleporter>();
                    Vector2 tpTargetRoom = ScriptUtilities.IsoGridToWorld(new Vector2(roomGridPos.x - 1, roomGridPos.y) * roomSpacing);
                    tp.targetLocation = tpTargetRoom + ScriptUtilities.IsoGridToWorld(new Vector2(9, 4));
                    doorPrefab.transform.SetParent(rmc.transform);
                    dc.AddDoor(doorInteractible);
                    rmc.doorLeft = doorPrefab;
                }

                if (tr)
                {
                    GameObject doorPrefab = PlaceDoorTeleporter(roomOrigin, new RoomLayouts.Position(trPos), asset_library.door_teleporter[1]);
                    LockedDoor doorInteractible = doorPrefab.GetComponent<LockedDoor>() ?? doorPrefab.GetComponentInChildren<LockedDoor>();
                    tp = doorPrefab.GetComponent<PlayerTeleporter>() ?? doorPrefab.GetComponentInChildren<PlayerTeleporter>();
                    Vector2 tpTargetRoom = ScriptUtilities.IsoGridToWorld(new Vector2(roomGridPos.x, roomGridPos.y + 1) * roomSpacing);
                    tp.targetLocation = tpTargetRoom + ScriptUtilities.IsoGridToWorld(new Vector2(4, 1));
                    doorPrefab.transform.SetParent(rmc.transform);
                    dc.AddDoor(doorInteractible);
                    rmc.doorUp = doorPrefab;
                }

                if (br)
                {
                    GameObject doorPrefab = PlaceDoorTeleporter(roomOrigin, new RoomLayouts.Position(brPos), asset_library.door_teleporter[2]);
                    LockedDoor doorInteractible = doorPrefab.GetComponent<LockedDoor>() ?? doorPrefab.GetComponentInChildren<LockedDoor>();
                    tp = doorPrefab.GetComponent<PlayerTeleporter>() ?? doorPrefab.GetComponentInChildren<PlayerTeleporter>();
                    Vector2 tpTargetRoom = ScriptUtilities.IsoGridToWorld(new Vector2(roomGridPos.x + 1, roomGridPos.y) * roomSpacing);
                    doorPrefab.transform.SetParent(rmc.transform);
                    tp.targetLocation = tpTargetRoom + ScriptUtilities.IsoGridToWorld(new Vector2(1, 4));
                    dc.AddDoor(doorInteractible);
                    rmc.doorRight = doorPrefab;
                }

                if (bl)
                {
                    GameObject doorPrefab = PlaceDoorTeleporter(roomOrigin, new RoomLayouts.Position(blPos), asset_library.door_teleporter[3]);
                    LockedDoor doorInteractible = doorPrefab.GetComponent<LockedDoor>() ?? doorPrefab.GetComponentInChildren<LockedDoor>();
                    tp = doorPrefab.GetComponent<PlayerTeleporter>() ?? doorPrefab.GetComponentInChildren<PlayerTeleporter>();
                    Vector2 tpTargetRoom = ScriptUtilities.IsoGridToWorld(new Vector2(roomGridPos.x, roomGridPos.y - 1) * roomSpacing);
                    tp.targetLocation = tpTargetRoom + ScriptUtilities.IsoGridToWorld(new Vector2(4, 9));
                    doorPrefab.transform.SetParent(rmc.transform);
                    dc.AddDoor(doorInteractible);
                    rmc.doorDown = doorPrefab;
                }
                
            }
            /*
            if (room.Doors.TopLeft.x != 0 && room.Doors.TopLeft.y != 0)
            {
                dc.AddDoor(PlaceDoorTeleporter(roomOrigin, room.Doors.TopLeft, asset_library.door_teleporter[0]));
            }

            if (room.Doors.TopRight.x != 0 && room.Doors.TopRight.y != 0)
            {
                dc.AddDoor(PlaceDoorTeleporter(roomOrigin, room.Doors.TopRight, asset_library.door_teleporter[0]));
            }

            if (room.Doors.BottomLeft.x != 0 && room.Doors.BottomLeft.y != 0)
            {
                dc.AddDoor(PlaceDoorTeleporter(roomOrigin, room.Doors.BottomLeft, asset_library.door_teleporter[0]));
            }

            if (room.Doors.BottomRight.x != 0 && room.Doors.BottomRight.y != 0)
            {
                dc.AddDoor(PlaceDoorTeleporter(roomOrigin, room.Doors.BottomRight, asset_library.door_teleporter[0]));
            }*/

        }
        private GameObject PlaceDoorTeleporter(Vector2 roomOrigin, RoomLayouts.Position doorPos, GameObject doorPrefab)
        {
            Vector2 world = ScriptUtilities.IsoGridToWorld(new Vector2(doorPos.x, doorPos.y));
            GameObject doorRetPrefab = Instantiate(doorPrefab, world +
                roomOrigin + new Vector2(doorPrefab.transform.position.x, doorPrefab.transform.position.y),
                Quaternion.identity);
            if (doorRetPrefab == null)
            {
                Debug.LogError("Error placing door teleporter prefab");
            }

            //TODO: SET DOOR TP POSITIONS

            
            return doorRetPrefab;
        }
        
    }
}