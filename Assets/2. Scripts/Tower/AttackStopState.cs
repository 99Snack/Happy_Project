using UnityEngine;

public class AttackStopState : ITowerState
{
    private Tower tower;
    private AnimatorStateInfo stateInfo;

    public AttackStopState(Tower tower)
    {
        this.tower = tower;
    }

    public void Enter()
    {
        tower.state = statetest.attackstop;

        // Attack 상태 종료, Cooldown 상태 활성화
        tower.animator.SetBool(tower.hashIsAttacking, false);
        tower.animator.SetBool(tower.hashIsCooldown, true);

        // 타겟 초기화
        tower.currentTarget = null;
    }

    public void Update()
    {
        stateInfo = tower.animator.GetCurrentAnimatorStateInfo(0);

        // 애니메이션이 완료되면 Search 상태로 전환
        if (stateInfo.normalizedTime >= 0.95f)
        {
            tower.ChangeState(tower.SearchingState);
        }
    }

    public void Exit()
    {
        tower.animator.SetBool(tower.hashIsCooldown, false);
    }
}