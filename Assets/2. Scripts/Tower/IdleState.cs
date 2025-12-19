using UnityEngine;
using UnityEngine.Tilemaps;

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

        //tower.animator.SetBool("isAttacking", false);
        //tower.animator.SetBool("isAttackReady", false);
        tower.currentTarget = null;
    }

    public void Update()
    {
        Vector2Int baseCampTile = TileManager.Instance.allyBasePosition;
        MonsterMove nearest = TowerTargetDetector.Instance.FindNearestEnemyInRange(
            tower.Coord,
            tower.attackRange, // 사거리 그대로
            baseCampTile
        );

        if (nearest != null)
        {
            tower.currentTarget = nearest;
            tower.ChangeState(new SearchingState(tower));
        }
    }

    public void Exit() { }
}
