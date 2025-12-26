using UnityEngine;

public class AttackStopState : ITowerState
{
    private Tower tower;
    AnimatorStateInfo StateInfo;

    public AttackStopState(Tower tower)
    {
        this.tower = tower;
    }

    public void Enter()
    {
        //Debug.Log("attackStop ->");


        tower.animator.SetBool(tower.hashIsCooldown, true);
    }

    public void Update()
    {
        StateInfo = tower.animator.GetCurrentAnimatorStateInfo(0);

        if (tower.IsTargetInRange())
        {
            tower.ChangeState(new AttackingState(tower));
            return;
        }

        if (StateInfo.normalizedTime < 1.0f) return;

        tower.ChangeState(new SearchingState(tower));
    }

    public void Exit()
    {
        tower.animator.SetBool(tower.hashIsCooldown, false);
    }
}
