using UnityEngine;

public class AttackingState : ITowerState
{
    private Tower tower;

    public AttackingState(Tower tower)
    {
        this.tower = tower;
    }

    public void Enter()
    {
        tower.animator.SetTrigger("AttackTrigger");
    }

    public void Update()
    {
        tower.Attack();
    }

    public void Exit()
    {

    }
}
