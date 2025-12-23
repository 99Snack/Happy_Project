
using Mono.Cecil.Cil;

public class SpearTower : MeleeTower
{
    protected override void Start()
    {
        base.Start();

        IdleState = new IdleState(this);
        AttackStopState = new AttackStopState(this);

        if (Soldier != null) IsRotate = true;

        ChangeState(IdleState);
    }
}
