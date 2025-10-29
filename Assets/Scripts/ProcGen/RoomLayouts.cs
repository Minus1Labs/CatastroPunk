using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Windows;
using Unity.VisualScripting;


/*
Generated list of Rooms from json file. Example:

{
  "Rooms": [
    {
      "Doors": {
        "Entrance_neg": {
          "x": 4,
          "y": 0
        },
        "Entrance_pos": {
          "x": 10,
          "y": 4
        },
        "Exit_neg": {
          "x": 4,
          "y": 10
        },
        "Exit_pos": {
          "x": 0,
          "y": 4
        }
      },
      "EnemySpawns": [
        {"x": 0, "y": 0}, {"x": 0, "y": 9}, {"x": 2, "y": 8},
        {"x": 5, "y": 3}, {"x": 5, "y": 5}, {"x": 7, "y": 9}
      ],
      "Props": {
        "1x1": [{"x": 0, "y": 1}, {"x": 4, "y": 5}],
        "1x2": [{"x": 0, "y": 6}, {"x": 3, "y": 8}],
        "1x3": [],
        "2x1": [{"x": 0, "y": 3}],
        "3x1": [{"x": 5, "y": 1}],
        "2x2": [{"x": 7, "y": 7}]
      }
    }
  ]
}
*/
public static class RoomLayouts
{
    [Serializable]
    public struct Root
    {
        public List<Room> Rooms;
    }
    [Serializable]
    public struct Room
    {
        public Position RoomSize;
        public Doors Doors;
        public int DoorsLayout;
        public List<Position> EnemySpawns;
        public Props Props;
        public List<Puzzle> Puzzles;
    }
    [Serializable]
    public struct Doors
    {
        public Position TopLeft;
        public Position TopRight;
        public Position BottomLeft;
        public Position BottomRight;
    }
    [Serializable]
    public struct Props
    {
        public List<Position> _1x1;
        public List<Position> _1x2;
        public List<Position> _1x3;
        public List<Position> _2x1;
        public List<Position> _3x1;
        public List<Position> _2x2;
    }
    [Serializable]
    public struct Puzzle
    {
        public string Type;
        public List<Position> Positions;
        public List<int> Modifiers;
    }
    [Serializable]
    public struct Position
    {
        public int x;
        public int y;
        public Position(Vector2 pos)
        {
            x = (int)pos.x; y = (int)pos.y;
        }
    }
    public static List<Room> GetRoomsFromDisk()
    {
        string json = System.IO.File.ReadAllText("Assets/Scripts/ProcGen/RoomLayoutsJQ.json");
        Root jsonRoot = JsonUtility.FromJson<Root>(json);
        Debug.Log("Got " + jsonRoot.Rooms.Count + " rooms from disk");

        return jsonRoot.Rooms;
    }
}

public class LayoutDictionary
{
    public Dictionary<int, List<DoorWrapper>> udict = new Dictionary<int, List<DoorWrapper>>();

    public struct DoorWrapper
    {
        public int doorConfig;
        public RoomLayouts.Room rlo;

        public DoorWrapper(RoomLayouts.Room r)
        {
            rlo = r;
            doorConfig = r.DoorsLayout;
        }
    }
    public LayoutDictionary(List<RoomLayouts.Room> rooms)
    {
        for (int i = 0; i < 16; i++)
        {
            udict[i] = new List<DoorWrapper>();
        }

        foreach (RoomLayouts.Room rm in rooms)
        {
            DoorWrapper dw = new DoorWrapper(rm);
            List<DoorWrapper> ld = udict[dw.doorConfig];
            ld.Add(dw);
        }
    }

    public static int GetDoorConfig(bool d0, bool d1, bool d2, bool d3)
    {
        return (d0 ? 1 : 0) + (d1 ? 2 : 0) + (d2 ? 4 : 0) + (d3 ? 8 : 0);
    }
}