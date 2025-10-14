using Unity.Mathematics;
using UnityEngine;

public static class RandomManager
{
    public static int Seed { get; set; }

    public static void SetSeed(int seed) {
        Seed = seed;
        UnityEngine.Random.InitState(seed);
    }

    public static void Reapply() {
        UnityEngine.Random.InitState(Seed);
    }

}