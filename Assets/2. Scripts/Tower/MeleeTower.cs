
public class MeleeTower : Tower
{
    public override void ApplyAugment(AugmentData augment)
    {
        // Tag 체크를 먼저
        if (augment.Tag != 0 && augment.Tag != 1) return;

        // 조건부 증강
        if (augment.Category == 3)
        {
            if (!appliedConditionAugments.Contains(augment.Index))
            {
                AddConditionAugment(augment);
                appliedConditionAugments.Add(augment.Index);
                UpdateConditionAugment();
            }
        }
        else
        {
            // 일반 능력치 증강
            if (augment.Category == 1)
            {
                UpdateStatus(augment);
            }
        }
    }

    public override int CalcAttackOfficial()
    {
        return Data.Attack * Data.HitCount;
    }
}
