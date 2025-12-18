using UnityEngine;

public class DeBuff
{
    public int DebuffID; // 디버프 ID (2자리) (01~99)
    public int Type; // 디버프 타입 (1 = 슬로우, 2 = 방어력 감소)
    public float DebuffPower; // 디버프 수치
    public float Duration; // 지속 시간 (1.0 = 1초)
    public string EffectResource; // 타워명_등급_이펙트_번호
}
