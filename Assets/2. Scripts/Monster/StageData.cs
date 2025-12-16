    using UnityEngine;

[System.Serializable]
public class StageData
{
    [Header("스폰 순서 (0:꽃, 1:박쥐, 2:벌)")]
    public int[] SpawnOrder;

    public int[] GetSpawnOrder()
    {
        return SpawnOrder;
    }
}