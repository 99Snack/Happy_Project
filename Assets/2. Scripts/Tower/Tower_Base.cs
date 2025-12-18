using UnityEngine;

public class Tower_Base
{
    public int TowerID; // 타워 ID(6자리) (100000~299999)
    public string Name; // 타워 이름

    public int MainType; // 1 = 근접, 2 = 원거리
    public int SubType; //  1 = 단일, 2 = 광역, 3= 디버프
    public int AtkType; // 1 = 즉발, 2 = 투사체

    public string ModelResource; // 타워명_Img_번호
    public string SoundResource; // 타워명_Sound_번호

    public string Desc; // 타워명_설명_번호
}
