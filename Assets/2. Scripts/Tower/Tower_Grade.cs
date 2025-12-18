using UnityEngine;

public class Tower_Grade
{
    public int TowerID; // // 타워 ID(6자리) (100000~299999)
    public int Grade; // 1 = 1성, 2 = 2성, 3 = 3성

    // 공격 관련 능력치
    public int Attack; // 공격력
    public float AttackInterval; // 공격 속도 (1.0 = 1초에 1번 공격)
    public int Range; // 사거리 (1 = 8방향 기준 타일 1칸)
    public float AtkScale; // 공격 범위 (1.0 - 타일 1칸)
    public int ProjectileSpeed; // 투사체 속도
    public int HitCount; // 타격 수 (1 = 1개의 적)

    // 상태 이상 및 효과
    public int DebuffID; // 디버프 ID (2자리) (01~99)
    public float DebuffPower; // 디버프 수치 (1.0 = 100%)

    // 외형 및 이펙트
    public float Size; // 타워 크기 (기본 값 1.0)
    public string Effect; // 타워 이펙트  (주변 오오라)

}
