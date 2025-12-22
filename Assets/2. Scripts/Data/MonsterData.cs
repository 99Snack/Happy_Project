using UnityEngine;

/// <summary>
/// 몬스터 DB 스탯 데이터를 관리하는 클래스 
/// </summary>
[System.Serializable]
public class MonsterData
{
    #region 몬스터 스탯 
    [Header("기본 몬스터 정보")]
    public int MonsterID;           // 몬스터 ID
    public int MonsterType;         // 몬스터 타입(1=일반, 2=레어, 3=엘리트)
    public int MonsterRank;         // 몬스터 등급(1=1성, 2=2성, 3=3성)
    
    [Header("몬스터 전투 스탯 능력치")]
    public int AttackType;          // 공격 타입 유형(1=일반, 2=레어, 3=엘리트)
    public int Hp;                  // 최대 체력
    public int Defense;             // 방어력
    public int Atk;                 // 공격력
    public int AtkIntervalMs;       // 공격 간격 주기 (ms, 밀리초)
    public int AtkSpeed;            // 공격 속도
    
    [Header("이동 관련")]
    public int MoveSpeed;           // 이동 속도
    
    [Header("보상관련 리워드 정보")]
    public string RewardGroup;      // 리워드 보상 그룹
    
    public string MonsterResource;  // 몬스터 모델링 리소스 경로 
          
    #endregion
}
