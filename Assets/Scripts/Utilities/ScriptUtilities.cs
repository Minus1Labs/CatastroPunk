using UnityEngine;

public static class ScriptUtilities
{

    public static readonly Vector2 gridUpRight = new Vector2(2, 1).normalized;
    public static readonly Vector2 gridDownRight = new Vector2(2, -1).normalized;
    public static readonly Vector2 gridUpLeft = new Vector2(-2, 1).normalized;
    public static readonly Vector2 gridDownLeft = new Vector2(-2, -1).normalized;

    public static readonly Vector2[] gridDirArray = {
        new Vector2(2, 1).normalized,
        new Vector2(2, -1).normalized,
        new Vector2(-2, 1).normalized,
        new Vector2(-2, -1).normalized
    };

    public enum PLAYER_SPRITE_ZONES
    {
        FEET,
        LEGS,
        BODY1,
        BODY2,
        BODY3,
        EYES,
        HEAD,
        ABOVE_HEAD,
        ABOVE_BODY,
        TAIL,
        TAIL_LASER,
        MOUTH,
        AROUND_HEAD,
        OUTLINE
    }

    public struct Item_Sprite_Info
    {
        bool hasCharcterSprite;
        string spriteDescription;
        PLAYER_SPRITE_ZONES spriteZone;
        int spritePriority;
        bool spriteIsStackable;
        SpriteRenderer sprite;
    }


    public enum FLOOR_NAMES
    {
        LAB,
        CRUISE_SHIP,
        JUPITER_CITY_STREETS
    }

    public static readonly int gridScalar = 5;

    /*
    Returns the closest isometric-grid aligned vector to the 
    given input vector.
    */
    public static Vector2 SnapToGridDirection(Vector2 vec)
    {
        Vector2 inputNormalized = vec.normalized;
        Vector2 bestVector = gridDirArray[0];
        float bestDot = 0.0f;

        for (int i = 0; i < gridDirArray.Length; i++)
        {
            float tempDot = Vector2.Dot(inputNormalized, gridDirArray[i]);
            if (tempDot > bestDot)
            {
                bestDot = tempDot;
                bestVector = gridDirArray[i];
            }
        }

        return bestVector;
    }

    //We want to have grid +x be y = -x; grid +y to be y = x
    public static Vector2 IsoGridToWorld(Vector2 gridPos)
    {
        float dx = (1f / 2f) * 5f;    // tileWidth / 2 * scale = 2.5
        float dy = (0.5f / 2f) * 5f;  // tileHeight / 2 * scale = 1.25

        float wx = (gridPos.x + gridPos.y) * dx;
        float wy = (-gridPos.x + gridPos.y) * dy;
        return new Vector2(wx, wy);
    }

    public static Vector2 WorldToIsoGrid(Vector2 worldPos)
    {
        float dx = (1f / 2f) * 5f;    // 2.5
        float dy = (0.5f / 2f) * 5f;  // 1.25
    
        float gx = 0.5f * ((worldPos.x / dx) - (worldPos.y / dy));
        float gy = 0.5f * ((worldPos.x / dx) + (worldPos.y / dy));
    
        return new Vector2(gx, gy);
    }
}
