using UnityEngine;


public class SpearTower : MeleeTower
{
    protected override void Start()
    {
        base.Start();

        IdleState = new IdleState(this);
        AttackStopState = new AttackStopState(this);

        ChangeState(IdleState);

    }

    protected override void test()
    {
        base.test();

        Debug.Log("Spear");
    }
}
