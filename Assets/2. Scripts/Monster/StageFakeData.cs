    using UnityEngine;

[System.Serializable]
public class StageFakeData
{
    [Header("스폰 순서")]
    public int[] SpawnOrder;

    public int[] GetSpawnOrder()
    {
        return SpawnOrder;
    }
}