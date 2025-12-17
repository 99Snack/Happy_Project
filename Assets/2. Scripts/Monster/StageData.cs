    using UnityEngine;

[System.Serializable]
public class StageData
{
    [Header("스폰 순서")]
    public int[] SpawnOrder;

    public int[] GetSpawnOrder()
    {
        return SpawnOrder;
    }
}