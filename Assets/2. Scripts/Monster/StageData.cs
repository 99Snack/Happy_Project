using UnityEngine;

[System.Serializable]
public class StageData
{
    [Header("½ºÆù ¼ø¼­ (0:²É, 1:¹ÚÁã, 2:¹ú)")]
    public int[] SpawnOrder;

    public int[] GetSpawnOrder()
    {
        return SpawnOrder;
    }
}