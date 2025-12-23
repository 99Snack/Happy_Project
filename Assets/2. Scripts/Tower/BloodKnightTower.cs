
public class BloodKnightTower : MeleeTower
{
    protected override void Start()
    {
        base.Start();

        IdleState = new IdleState(this);
        AttackStopState = new AttackStopState(this);

        if (Soldier != null) IsRotate = true;

        ChangeState(IdleState);
    }

    public override void Attack()
    {

        if (currentTarget == null) return;

        int attackPower = CalcAttackPower();

        //monster.OnHit(attackPower);

        if (CanAttack())
        {
            ResetCooldown(Data.AttackInterval);
            animator.SetTrigger(hashAttack);
        }

        //디버프 관련 내용 들어가야함
    }
}
