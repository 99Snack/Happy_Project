using UnityEngine;

public class HarvestAugment : IOnHitAugment
{
    float ratio;
    public HarvestAugment(float ratio)
    {
        this.ratio = ratio;
    }

    public void OnHit(Tower owner, Monster target, AugmentData augment)
    {
        //보스 인지
        //if (target.IsBoss) return;

        ratio = augment.Value_N * 0.01f;
        float healthPercentage = target.currentHp / target.Data.Hp;

        if (healthPercentage <= ratio)
        {
            target.Die();
            Debug.Log($"{owner.name}의 증강 발동: 수확");
        }
    }
}
