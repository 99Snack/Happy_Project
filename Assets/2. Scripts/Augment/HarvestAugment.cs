using UnityEngine;

public class HarvestAugment : IOnHitAugment
{
    float ratio;
    public HarvestAugment(float ratio)
    {
        this.ratio = ratio * 0.01f;
    }

    public void OnHit(Tower owner, Monster target)
    {
        //보스 인지
        //if (target.IsBoss) return;

        float healthPercentage = target.currentHp / target.Data.Hp;

        if (healthPercentage <= ratio)
        {
            target.Die(owner);
            Debug.Log($"{owner.name}의 증강 발동: 수확");
        }
    }
}
