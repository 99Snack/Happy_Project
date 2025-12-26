using UnityEngine;

public class MineThrowerTower : RangeTower, IHitEffect
{
    protected override void Start()
    {
        base.Start();
        IdleState = new IdleState(this);
        AttackStopState = new AttackStopState(this);

        ChangeState(IdleState);

    }

    public override int CalcAttackOfficial()
    {
        return Data.Attack * Data.HitCount;
    }

    public override void Attack()
    {
        if (currentTarget == null) return;

        HitEffect();

        if (onHitAugs.Count > 0)
        {
            foreach (var aug in onHitAugs)
            {
                aug.OnHit(this, currentTarget);
            }
        }
        else
        {
            currentTarget.TakeDamage(atkPower.finalStat);
        }

        if (CanAttack())
        {
            ResetCooldown(Data.AttackInterval);
            animator.SetTrigger(hashAttack);
        }

    }
    public override void ApplyAugment(AugmentData augment)
    {
        base.ApplyAugment(augment);

        if (augment.Tag != 3) return;

        if (augment.Category == 3)
        {
            UpdateConditionAugment(augment);
        }
    }

    public void HitEffect()
    {
        ObjectPoolManager.Instance.SpawnFromPool("minethrower", currentTarget.transform.position, Quaternion.identity);
    }
}
