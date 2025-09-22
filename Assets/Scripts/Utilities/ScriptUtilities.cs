using UnityEngine;

public static class ScriptUtilities {

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
}
