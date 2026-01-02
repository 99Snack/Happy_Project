using UnityEngine;

public class KnightTower : MeleeTower, IAreaAttack
{
    protected override void Start()
    {
        base.Start();
        SetState(this);
        ChangeState(IdleState);
    }

    public override int CalcAttackOfficial()
    {
        return Data.Attack * Data.HitCount;
    }

    // 애니메이션 이벤트에서 호출될 실제 데미지 처리
    public override void ExecuteDamage()
    {
        if (currentTarget == null) return;
        AreaAttack();
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
            UpdateConditionAugment();
        }
    }
}