
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
        tower.state = statetest.idle;
        tower.currentTarget = null;

        tower.animator.SetBool(tower.hashIsReady, false);
        tower.animator.SetBool(tower.hashIsAttacking, false);
        tower.animator.SetBool(tower.hashIsCooldown, false);
    }

    public void Update()
    {
        if (tower.MyTile.Type == TileInfo.TYPE.Wall)
        {
            tower.ChangeState(tower.SearchingState);
        }
    }

    public void Exit()
    {
        // Idle 상태 종료 시 특별한 처리 없음
    }
}
