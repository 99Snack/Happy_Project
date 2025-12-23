
using UnityEngine;

public class MageTower : RangeTower, IAreaAttack
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

    public override int CalcAttackPower()
    {
        //1회 공격 피해량 = 타워 공격력 x( 1 – 몬스터 방어력 )
        //todo : return Data.Attack * (1 - monster.Data.Defense);
        return Data.Attack;
    }

    public override void Attack()
    {
        if (currentTarget == null) return;

        finalAttackPower = CalcAttackPower();

        //광역 공격
        AreaAttack();

        if (CanAttack())
        {
            ResetCooldown(Data.AttackInterval);
            animator.SetTrigger(hashAttack);
        }

    }

    public void HitEffectPlay(){
        ObjectPoolManager.Instance.SpawnFromPool("mage", currentTarget.transform.position, Quaternion.identity);
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
                    m.TakeDamage(finalAttackPower);
                }
            }
        }
    }

}
