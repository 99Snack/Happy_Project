using UnityEngine;

public class AttackStopState : ITowerState
{
    private Tower tower;

    public AttackStopState(Tower tower)
    {
        this.tower = tower;
    }

    public void Enter()
    {
        Debug.Log("[타워] AttackStop 상태 진입");
        tower.animator.SetTrigger("isAttackStop");

        // Animator Event에서 Tower.OnAttackStopEnd() 호출
    }

    public void Update() {
        tower.ChangeState(new IdleState(tower));
    }
    public void Exit() { }
}
