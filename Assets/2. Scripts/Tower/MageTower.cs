
using UnityEngine;

public class MageTower : RangeTower, IAreaAttack, IHitEffect
{
    private int finalAttackPower;

    protected override void Start()
    {
        base.Start();

        IdleState = new IdleState(this);
        AttackStopState = new AttackStopState(this);

        ChangeState(IdleState);
    }

    public override int CalcAttackOfficial()
    {
        return Data.Attack;
    }

    public override void Attack()
    {
        if (currentTarget == null) return;

        //광역 공격
        AreaAttack();

        if (CanAttack())
        {
            ResetCooldown(Data.AttackInterval);
            animator.SetTrigger(hashAttack);
        }

    }

    public void AreaAttack()
    {
        Vector3 center = currentTarget.transform.position;
        Collider[] cols = Physics.OverlapSphere(center, Data.AtkScale, monsterLayer);
        if (cols.Length > 0)
        {
            foreach (var target in cols)
            {
                Monster m = target.GetComponent<Monster>();
                if (m != null)
                {
                    m.TakeDamage(atkPower.finalStat);
                }
            }
        }
    }

    public override void ApplyAugment(AugmentData augment)
    {
        base.ApplyAugment(augment);

        if (augment.Tag != 4) return;

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
