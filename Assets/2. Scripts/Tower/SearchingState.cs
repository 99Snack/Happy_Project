using UnityEngine;
using UnityEngine.Tilemaps;

public class SearchingState : ITowerState
{
    private Tower tower;

    public SearchingState(Tower tower)
    {
        this.tower = tower;
    }

    public void Enter()
    {
        Debug.Log("[타워] AttackReady 상태 진입");
        tower.animator.SetBool("isAttackReady", true);
        //tower.animator.SetBool("isAttacking", false);
    }

    public void Update()
    {
        if (tower.currentTarget == null)
        {
            tower.ChangeState(new IdleState(tower));
            return;
        }

        // 사거리 체크

        int enemyX = Mathf.FloorToInt(tower.currentTarget.transform.position.x);
        int enemyY = Mathf.FloorToInt(tower.currentTarget.transform.position.z);

        Vector2Int enemyTile = new Vector2Int(enemyX, enemyY);


        int dx = Mathf.Abs(tower.Coord.x - enemyTile.x);
        int dy = Mathf.Abs(tower.Coord.y - enemyTile.y);
        int distance = Mathf.Max(dx, dy);

        if (distance <= tower.attackRange)
        {
            tower.animator.SetTrigger("AttackTrigger");
            tower.ChangeState(new AttackingState(tower));
        }
        else
        {
            tower.ChangeState(new IdleState(tower));
        }
    }

    public void Exit()
    {
        tower.animator.SetBool("isAttackReady", false);
    }
}
