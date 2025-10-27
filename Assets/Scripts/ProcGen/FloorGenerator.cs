using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace ProcGen
{
    public class FloorGenerator : MonoBehaviour
    {
        public int mainPathSize = 7;
        public int maxRooms = 10;
        [Range(0f, 1f)] public float connectedness = 0.5f;
        [Range(0f, 1f)] public float doorChance = 1f;
        [Range(0f, 1f)] public float doorChanceMultiplier = 0.5f;
        public RoomTile start;
        public RoomTile end;
        [SerializeField] private AssetLibrary asset_library;
        [SerializeField] Tilemap floorTilemap;

        [SerializeField] GameObject rmgPrefab;

        [SerializeField] private int gridSize = 15;

        //TODO: Ensure this never goes OOB. 
        public RoomTile[,] grid;

        public struct PathRoom
        {
            public int x;
            public int y;
            public RoomTile roomTile;
            public PathRoom(int a, int b, RoomTile rt)
            {
                x = a;
                y = b;
                roomTile = rt;
            }
        }

        private List<PathRoom> mainPathRooms = new List<PathRoom>();

        private List<PathRoom> divergentPathRooms = new List<PathRoom>();

        //Awake because we want to verify the layouts, prefabs, and set random before starting.
        public void Awake()
        {
            SetLabSE();
            if (start == null || end == null)
            {
                Debug.Log("error with start/end");
                return;
            }

            InitGrid();

            GenerateMainPath();
            //GenerateDivergentPaths();

            PrintGridDebug();

            LayoutDictionary layoutDictionary = new LayoutDictionary(RoomLayouts.GetRoomsFromDisk());

            AssignRoomLayouts(mainPathRooms, layoutDictionary);
            AssignRoomLayouts(divergentPathRooms, layoutDictionary);

            GameObject rmgGO = Instantiate(rmgPrefab);
            RoomGenerator rmg = rmgGO.GetComponent<RoomGenerator>();
            rmg.SetVars(asset_library, mainPathRooms, floorTilemap);
            rmg.DoRoomGen();
        }

        private void InitGrid()
        {
            grid = new RoomTile[gridSize, gridSize];
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    grid[i, j] = new RoomTile();
                }
            }
        }

        private void GenerateMainPath()
        {
            grid[gridSize / 2, gridSize / 2] = start;
            int roomCount = 1;
            RoomTile currentTile = grid[gridSize / 2, gridSize / 2];
            Vector2Int currentPos = new Vector2Int(gridSize / 2, gridSize / 2);

            mainPathRooms.Add(new PathRoom(currentPos.x, currentPos.y, currentTile));
            int direction = 1; //Tuberoom only has an exit on side 1
            currentTile.SetDoor(direction, true);
            int nextDirection = -1;
            
            while (roomCount < mainPathSize)
            {
                currentPos = ShiftDirection(direction, currentPos);

                int oppositeDirection = direction + 2;
                if (oppositeDirection > 3) { oppositeDirection -= 4; }

                //pick a random direction except for the opposite direction
                nextDirection = UnityEngine.Random.Range(0, 3);
                if (nextDirection == 2) { nextDirection++; }
                nextDirection += direction;
                if (nextDirection > 3) { nextDirection -= 4; }

                //if turning a random direction would make us run into an existing tile, then continue straight
                //if the new direction would touch an existing tile, then continue straight
                Vector2Int tempPos = ShiftDirection(nextDirection, currentPos);
                if (grid[tempPos.x, tempPos.y].ESCAPE_CONDITION != RoomTile.ROOM_ESCAPE_CONDITION.NONE ||
                    !AdjacentIsClear(tempPos, currentPos))
                {
                    nextDirection = direction;
                }

                //TODO: Allow other shapes
                currentTile = new RoomTile(false, false, false, false, RoomTile.ROOM_ESCAPE_CONDITION.UNASSIGNED, RoomTile.ROOM_SHAPE.SQUARE);

                //Set doors for the new RoomTile, and add it to data structures
                currentTile.SetDoor(oppositeDirection, true);
                currentTile.SetDoor(nextDirection, true);
                grid[currentPos.x, currentPos.y] = currentTile;
                mainPathRooms.Add(new PathRoom(currentPos.x, currentPos.y, currentTile));

                roomCount++;
                direction = nextDirection;
            }
            currentTile.SetDoor(nextDirection, false);
            currentTile.ESCAPE_CONDITION = RoomTile.ROOM_ESCAPE_CONDITION.BOSS;
        }

        private void AssignRoomLayouts(List<PathRoom> pathRoomList, LayoutDictionary layoutDictionary)
        {
            foreach (PathRoom pr in pathRoomList)
            {
                if (pr.roomTile.ESCAPE_CONDITION == RoomTile.ROOM_ESCAPE_CONDITION.START ||
                    pr.roomTile.ESCAPE_CONDITION == RoomTile.ROOM_ESCAPE_CONDITION.BOSS)
                {
                    continue;
                }
                if (layoutDictionary.udict[pr.roomTile.doorConfig] == null || layoutDictionary.udict[pr.roomTile.doorConfig].Count < 1)
                {
                    Debug.Log("No layouts found for PathRoom at: " + pr.x + ", " + pr.y + ". ");
                    //TODO: use fallback layout
                    continue;
                }
                int layoutIndex = UnityEngine.Random.Range(0, layoutDictionary.udict[pr.roomTile.doorConfig].Count);
                pr.roomTile.layout = layoutDictionary.udict[pr.roomTile.doorConfig][layoutIndex].rlo;

            }
        }


        private Vector2Int ShiftDirection(int direction, Vector2Int pos)
        {
            Vector2Int ret = new Vector2Int(pos.x, pos.y);
            if (direction == 0) { ret.x--; }
            else if (direction == 1) { ret.y++; }
            else if (direction == 2) { ret.x++; }
            else { ret.y--; }

            return ret;
        }

        private bool AdjacentIsClear(Vector2Int gridPos, Vector2Int previous)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2Int tp = ShiftDirection(i, gridPos);
                if (tp != previous && grid[tp.x, tp.y].ESCAPE_CONDITION != RoomTile.ROOM_ESCAPE_CONDITION.NONE)
                {
                    return false;
                }
            }
            return true;
        }

        //hardcoded lab start and end rooms
        private void SetLabSE()
        {
            start = new RoomTile(false, true, false, false, RoomTile.ROOM_ESCAPE_CONDITION.START, RoomTile.ROOM_SHAPE.SQUARE);
            end = new RoomTile(true, true, true, true, RoomTile.ROOM_ESCAPE_CONDITION.BOSS, RoomTile.ROOM_SHAPE.FOUR_SQUARE);
        }

        private void PrintGridDebug()
        {
            String line = "";
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    line += RoomTile.EscapeCondChar(grid[i, j].ESCAPE_CONDITION) + " ";
                }
                line += "\n";
            }
            Debug.Log(line);
        }

    }

    
}