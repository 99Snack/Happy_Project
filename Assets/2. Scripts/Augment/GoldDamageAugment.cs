using UnityEngine;

public class GoldDamageAugment : IStatusCheckAugment
{
    private int lastBonus = 0;
    public void UpdateStatus(Tower owner, AugmentData augment)
    {
        int currentGold = GameManager.Instance.Gold;
        int unitGold = augment.Value_M; 
        float damagePerUnit = augment.Value_N; 

        int newBonus = Mathf.FloorToInt((currentGold / unitGold) * damagePerUnit);

        if (newBonus != lastBonus)
        {
            owner.atkPower.additiveStat -= lastBonus;

            owner.atkPower.additiveStat += newBonus;
            lastBonus = newBonus;
        }

    }
}
