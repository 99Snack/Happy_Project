using UnityEngine;
public class IdleState : ITowerState
{
    private Tower tower;

    public IdleState(Tower tower)
    {
        this.tower = tower;
    }

    public void Enter()
    {
        Debug.Log("[타워] Idle 상태 진입");
        tower.ChangeState(new SearchingState(tower));
    }

    public void Update() { }

    public void Exit() { }
}
