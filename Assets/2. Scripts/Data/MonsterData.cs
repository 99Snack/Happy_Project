[System.Serializable]
public class MonsterData

{
    public int MonsterId;            // 몬스터 ID
    public int MonsterType;          // 1=일반, 2=레어, 3=엘리트
    public int MonsterRank;          // 1=1성, 2=2성
    public int AttackType;           // 1=근접, 2=원거리
    public int Hp;                   // 체력
    public float Defense;              // 방어력(%)
    public int MoveSpeed;            // 이동 속도
    public int Atk;                  // 공격력
    public int AtkInterval_ms;       // 공격 주기
    public int AtkSpeed;             // 공격 속도
    public int RewardGroup;       // 리워드 그룹
    public string MonsterName;    // 몬스터명 스트링키
    public string MonsterResource;   // 모델링 리소스
}
