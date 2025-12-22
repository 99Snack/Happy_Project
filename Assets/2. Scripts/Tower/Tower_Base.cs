public class Tower_Base
{
    public int TowerID; // 타워 ID(6자리) (100000~299999)

    public string Name; // 타워 이름
    public string Desc; // 타워명_설명_번호

    public int MainType; // 1 = 근접, 2 = 원거리
    public int SubType; //  1 = 단일, 2 = 광역, 3= 디버프
    public int AtkType; // 1 = 즉발, 2 = 투사체

    public int Attack;          //공격력
    public float AttackInterval;  //공격 속도 (1=1초에 1번공격)
    public int Range;           //사거리(1 = 8방향 기준 타일 1칸
    public int AtkScale;        //공격 범위 (1 = 1m)
    public int ProjectileSpeed; //투사체 속도 (근접은 null)
    public int DebuffID;        //디버프 ID(2자리) (01~99)
    public int HitCount;        //타격 수(1회 공격에서 적용하는 피해 횟수)
    public float Size;          //타워 크기 기본 값(1)
    public int price;           //타워 판매 가격 1 = 1골드    

    public string ModelResource; // 타워명_Img_번호
    public string SoundResource; // 타워명_Sound_번호

    //public Tower_Base(Tower_Base copy)
    //{
    //    // 기본 정보
    //    TowerID = copy.TowerID;
    //    Name = copy.Name;
    //    Desc = copy.Desc;

    //    // 타입 정보
    //    MainType = copy.MainType;
    //    SubType = copy.SubType;
    //    AtkType = copy.AtkType;

    //    // 전투 능력치
    //    Attack = copy.Attack;
    //    AttackInterval = copy.AttackInterval;
    //    Range = copy.Range;
    //    AtkScale = copy.AtkScale;
    //    ProjectileSpeed = copy.ProjectileSpeed;
    //    HitCount = copy.HitCount;
    //    Size = copy.Size;

    //    // 디버프 및 리소스
    //    DebuffID = copy.DebuffID;
    //    ModelResource = copy.ModelResource;
    //    SoundResource = copy.SoundResource;
    //}
}
