using UnityEngine;

public class MineThrowerTower : RangeTower
{
    [SerializeField] private GameObject hitEffect;

    protected override void Start()
    {
        base.Start();
        IdleState = new IdleState(this);
        AttackStopState = new AttackStopState(this);

        ChangeState(IdleState);

    }

    //protected override void Update()
    //{
    //    base.Update();
    //}

    public override void Attack(MonsterMove monster)
    {
        if (monster == null) return;

        int attackPower = CalcAttackPower();
        monster.OnHit(attackPower);

        //타격 이펙트 몬스터자리에 생성
        Instantiate(hitEffect, monster.transform.position, Quaternion.identity);

        if (attackCooldown <= 0f)
        {
            attackCooldown = Data.AttackInterval;
            //shooter.Shoot(currentTarget, 1, Data.HitCount);

            animator.SetTrigger(hashAttack);
        }
    }

}
