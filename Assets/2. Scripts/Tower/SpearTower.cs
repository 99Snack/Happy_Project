
public class SpearTower : MeleeTower
{
    protected override void Start()
    {
        base.Start();
        SetState(this);
        ChangeState(IdleState);
    }

    // ExecuteDamage는 base 클래스에서 처리됨 (단일 공격)

    public override void ApplyAugment(AugmentData augment)
    {
        base.ApplyAugment(augment);

        if (augment.Tag != 3) return;

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
            if (augment.Category == 1)
            {
                UpdateStatus(augment);
            }
        }
    }

}
