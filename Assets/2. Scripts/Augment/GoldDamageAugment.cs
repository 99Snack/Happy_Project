using UnityEngine;

public class GoldDamageAugment : IStatusCheckAugment
{
    private int lastBonus = 0;
    private AugmentData data;
    public GoldDamageAugment(AugmentData data){
        this.data = data;
    }
    public void UpdateStatus(Tower owner)
    {
        int currentGold = GameManager.Instance.Gold;
        int unitGold = data.Value_M; 
        float damagePerUnit = data.Value_N; 

        int newBonus = Mathf.FloorToInt((currentGold / unitGold) * damagePerUnit);

        if (newBonus != lastBonus)
        {
            owner.atkPower.additiveStat -= lastBonus;

            owner.atkPower.additiveStat += newBonus;
            lastBonus = newBonus;
        }

    }
}
