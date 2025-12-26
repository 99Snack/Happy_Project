
public class SpearTower : MeleeTower
{
    protected override void Start()
    {
        base.Start();
        SetState(this);
        ChangeState(IdleState);
    }

    public override void Attack()
    {
        if (currentTarget == null) return;

        // OnHit 증강 적용
        if (onHitAugs.Count > 0)
        {
            foreach (var aug in onHitAugs)
            {
                aug.OnHit(this, currentTarget);
            }
        }
        else
        {
            currentTarget.TakeDamage(atkPower.finalStat, this);
        }
    }

    public override void ApplyAugment(AugmentData augment)
    {
        // Tag 체크: 0(공통), 1(근거리), 3(창병)
        if (augment.Tag != 0 && augment.Tag != 1 && augment.Tag != 3) return;

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
}
