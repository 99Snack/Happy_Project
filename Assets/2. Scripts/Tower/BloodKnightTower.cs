
using UnityEngine;

public class BloodKnightTower : MeleeTower, IAreaAttack, IHitEffect
{
    protected override void Start()
    {
        base.Start();
        SetState(this);
        ChangeState(IdleState);
    }

    public override int CalcAttackOfficial()
    {
        return Mathf.FloorToInt(Data.Attack * 0.8f);
    }

    public override void ExecuteDamage()
    {
        if (currentTarget == null) return;

        AreaAttack();
        HitEffect();
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
                    m.TakeDamage(atkPower.finalStat, this);

                    DebuffData debuff = DataManager.Instance.DebuffData[Data.DebuffID];
                    if (debuff != null)
                    {
                        m.TakeDebuff(debuff);
                    }
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
            UpdateConditionAugment();
        }
    }

    public void HitEffect()
    {
        if (currentTarget != null)
        {
            ObjectPoolManager.Instance.SpawnFromPool("bloodknight", currentTarget.transform.position, Quaternion.identity);
        }
    }
}
