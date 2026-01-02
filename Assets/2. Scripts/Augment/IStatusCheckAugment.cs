
//상태 체크 (고립 시 강화, 주변 버프 등)
public interface IStatusCheckAugment
{
    void UpdateStatus(Tower owner);
}