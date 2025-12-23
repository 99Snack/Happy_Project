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

    public override void Attack(Monster monster)
    {
        base.Attack(monster);
    
        //타격 이펙트 몬스터자리에 생성
        Instantiate(hitEffect, monster.transform.position, Quaternion.identity);
    }

}
