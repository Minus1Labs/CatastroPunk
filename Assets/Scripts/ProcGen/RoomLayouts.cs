using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Windows;


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
        public Doors Doors;
        // If using JsonUtility, rename your JSON key to "EnemySpawns"
        public List<Position> EnemySpawns;
        public Props Props;
    }
    [Serializable]
    public struct Doors
    {
        public Position Entrance_neg;
        public Position Entrance_pos;
        public Position Exit_neg;
        public Position Exit_pos;
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
    public struct Position
    {
        public int x;
        public int y;
    }
    public static List<Room> GetRoomsFromDisk()
    {
        string json = System.IO.File.ReadAllText("Assets/Scripts/ProcGen/RoomLayouts.json");
        Root jsonRoot = JsonUtility.FromJson<Root>(json);
        Debug.Log("Got " + jsonRoot.Rooms.Count + " rooms from disk");
        
        return jsonRoot.Rooms;
    }
}