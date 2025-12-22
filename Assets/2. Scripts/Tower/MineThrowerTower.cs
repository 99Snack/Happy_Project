using UnityEngine;

public class MineThrowerTower : RangeTower
{
    protected override void Start()
    {
        base.Start();
        IdleState = new IdleState(this);
        AttackStopState = new AttackStopState(this);

        ChangeState(IdleState);

    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Attack()
    {
        if (currentTarget != null && attackCooldown <= 0f)
        {
            if (animator != null)
            {
                animator.SetTrigger("AttackTrigger");
            }

          
            base.Attack();
        }
    }
}
