using UnityEngine;

public class IceMageTower : RangeTower, IAreaAttack, IHitEffect
{
    private int finalAttackPower;

    protected override void Start()
    {
        base.Start();

        IdleState = new IdleState(this);
        AttackStopState = new AttackStopState(this);

        if (Soldier != null)
        {
            IsRotate = true;
            animator.applyRootMotion = true;
        }

        ChangeState(IdleState);
    }

    public override int CalcAttackOfficial()
    {
        return Mathf.FloorToInt(Data.Attack * 0.8f);
    }

    public override void Attack()
    {
        if (currentTarget == null) return;

        //광역 공격
        AreaAttack();

        HitEffect();

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
                    //공격
                    m.TakeDamage(atkPower.finalStat);
                    //디버프
                    DebuffData debuff = DataManager.Instance.DebuffData[Data.DebuffID];
                    m.TakeDebuff(debuff);
                }
            }
        }
    }

    public override void ApplyAugment(AugmentData augment)
    {
        base.ApplyAugment(augment);

        if (augment.Tag != 5) return;

        if (augment.Category == 3)
        {
            UpdateConditionAugment(augment);
        }
    }

    public void HitEffect()
    {
        ObjectPoolManager.Instance.SpawnFromPool("icemage", currentTarget.transform.position, Quaternion.identity);
    }
}
