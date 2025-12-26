// 타격 시 (전이, 흡혈, 디버프 등)
public interface IOnHitAugment
{
    void OnHit(Tower owner, Monster target);
}

