using UnityEngine;

// 인스펙터에서 스테이지(배열) 별로 스폰 순서 데이터 설정 가능하게 하려는 용도 
[System.Serializable]
public class StageFakeData
{
    [Header("몬스터 프리팹 배열의 스폰 인덱스 순서")]
    public int[] SpawnOrder;

    public int[] GetSpawnOrder()
    {
        return SpawnOrder;
    }
}