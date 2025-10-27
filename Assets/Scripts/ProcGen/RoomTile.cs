using UnityEngine;

namespace ProcGen
{
    public class RoomTile
    {
        /* Door numbers
                   _/\_
               0 _/    \_ 1
                /        \
                \_      _/
               3  \_  _/  2
                    \/

        */
        public bool[] door = new bool[4];
        public int doorConfig;
        public ROOM_ESCAPE_CONDITION ESCAPE_CONDITION;
        public ROOM_SHAPE SHAPE;
        public bool ownedByOtherTile = false;
        public RoomTile otherTile;
        public RoomLayouts.Room layout;

        public RoomTile(bool d0, bool d1, bool d2, bool d3, ROOM_ESCAPE_CONDITION ec, ROOM_SHAPE s)
        {
            door[0] = d0; door[1] = d1; door[2] = d2; door[3] = d3;
            doorConfig = LayoutDictionary.GetDoorConfig(d0, d1, d2, d3);
            ESCAPE_CONDITION = ec;
            SHAPE = s;
        }

        public void SetDoor(int doornum, bool state)
        {
            if (doornum < 0 || doornum > 4)
            {
                Debug.Log("more than 4 doors unimplemented");
                return;
            }
            door[doornum] = state;
            doorConfig = LayoutDictionary.GetDoorConfig(door[0], door[1], door[2], door[3]);
        }

        public RoomTile()
        {
            ESCAPE_CONDITION = ROOM_ESCAPE_CONDITION.NONE;
            SHAPE = ROOM_SHAPE.SQUARE;
        }

        public static char EscapeCondChar(ROOM_ESCAPE_CONDITION ec)
        {
            if (ec == ROOM_ESCAPE_CONDITION.START)
            {
                return 's';
            }
            else if (ec == ROOM_ESCAPE_CONDITION.BOSS)
            {
                return 'e';
            }
            else if (ec == ROOM_ESCAPE_CONDITION.KILL_ALL)
            {
                return 'K';
            }
            else if (ec == ROOM_ESCAPE_CONDITION.KILL_SELECT)
            {
                return 'k';
            }
            else if (ec == ROOM_ESCAPE_CONDITION.PUZZLE_ONLY)
            {
                return 'P';
            }
            else if (ec == ROOM_ESCAPE_CONDITION.PUZZLE_WITH_ENEMIES)
            {
                return 'p';
            }
            else if (ec == ROOM_ESCAPE_CONDITION.CHILL)
            {
                return 'c';
            }
            else if (ec == ROOM_ESCAPE_CONDITION.UNASSIGNED)
            {
                return '?';
            }
            return ' ';
        }

        /*
            SQUARE          - 10x10
            HALLWAY_SMALL_X - 10x(<10)
            HALLWAY_SMALL_Y - (<10)x10
            DOUBLE_X        - 20x10
            DOUBLE_Y        - 10x20
            FOUR_SQUARE     - 20x20
        */
        public enum ROOM_SHAPE
        {
            SQUARE,
            HALLWAY_SMALL_X,
            HALLWAY_SMALL_Y,
            DOUBLE_X,
            DOUBLE_Y,
            FOUR_SQUARE,
            CONTINUED
        }

        /*
        NONE                - " "
        UNASSIGNED          - "?"
        START               - "s"
        BOSS                - "e"
        KILL_ALL            - "K"
        KILL_SELECT         - "k"
        PUZZLE_ONLY         - "P"
        PUZZLE_WITH_ENEMIES - "p"
        CHILL               - "c"
        */
        public enum ROOM_ESCAPE_CONDITION
        {
            NONE,
            UNASSIGNED,
            START,
            BOSS,
            KILL_ALL,
            KILL_SELECT,
            PUZZLE_ONLY,
            PUZZLE_WITH_ENEMIES,
            CHILL
        }

    }
}