
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
        Debug.Log("idle ->");

        //tower.animator.SetBool("isAttacking", false);
        //tower.animator.SetBool("isAttackReady", false);
        tower.currentTarget = null;
    }

    public void Update()
    {
        if (tower.MyTile.Type == TileInfo.TYPE.Wall)
        {
            tower.ChangeState(new SearchingState(tower));
        }

        //Vector2Int baseCampTile = TileManager.Instance.allyBasePosition;
        //MonsterMove nearest = TowerTargetDetector.Instance.FindNearestEnemyInRange(
        //    tower.Coord,
        //    tower.Data.Range, // 사거리 그대로
        //    baseCampTile
        //);

        //if (nearest != null)
        //{
        //    tower.currentTarget = nearest;
        //    tower.ChangeState(new SearchingState(tower));
        //}
    }

    public void Exit() { }
}
