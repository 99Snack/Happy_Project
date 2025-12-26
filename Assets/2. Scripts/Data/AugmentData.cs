public class AugmentData
{
    public int Index;                // 증강 ID
    public int Tier;                 // 1:실버, 2:골드, 3:프리즘
    public int Tag;                  // 0:공용, 1:근접, 2:원거리, 3:단일, 4:광역, 5:디버프
    public int Plus_Factor;          //요소( 1 : 공격력, 2 : 베이스 캠프 체력, 3 : 골드, 4: 조건부 )
    public int Category;             // 1:능력치, 2:재화, 3:조건부
    public int Value_N;              //증강에서의 n값
    public int Value_M;              //증강에서의 m값
    public float Grow_Value;         //증강 성장 계수 n (1 + {stage-1} x n) = 스테이지 별 증강 값
    public string Name_STR;           //증강 이름 스트링키
    public string Desc_STR;           //증강 설명 스트링키
    public string Icon_Resource;      //증강 아이콘 리소스

    public float CalcGrowValue(int stage)
    {
        //n(1 + { stage - 1} x n) = 스테이지 별 증강 값
        return (1 + (stage - 1) * Grow_Value);
    }
}
