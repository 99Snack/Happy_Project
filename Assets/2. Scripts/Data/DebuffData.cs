using UnityEngine;

public class DebuffData
{
    public int DebuffId;             // 디버프 ID (10001~99999)
    public int Type;                 // 1=슬로우, 2=방어력 감소
    public float DebuffPower;        // 수치 (1.0 = 100%)
    public float Duration;           // 지속 시간
    public string EffectResource;    // 디버프 리소스
}
