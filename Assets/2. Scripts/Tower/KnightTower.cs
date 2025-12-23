using UnityEngine;

public class KnightTower : MeleeTower
{
    public LayerMask monsterLayer;

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

    public override int CalcAttackPower(Monster monster)
    {
        //1회 공격 피해량 = 타워 공격력 x( 1 – 몬스터 방어력 )
        //todo : return Data.Attack * (1 - monster.Data.Defense);
        return Data.Attack;
    }

    public override void Attack(Monster monster)
    {
        if (monster == null) return;

        int attackPower = CalcAttackPower(monster);

        //광역 공격
        Vector3 center = monster.transform.position;
        Collider[] cols = Physics.OverlapSphere(center, Data.AtkScale, monsterLayer);
        if (cols.Length > 0)
        {
            foreach (var target in cols)
            {
                Monster m = target.GetComponent<Monster>();
                if (m != null)
                {
                    m.TakeDamage(attackPower);
                }
            }
        }

        if (CanAttack())
        {
            ResetCooldown(Data.AttackInterval);
            animator.SetTrigger(hashAttack);
        }

    }
}
