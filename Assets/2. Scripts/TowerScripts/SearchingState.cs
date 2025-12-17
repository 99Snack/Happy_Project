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
        tower.animator.SetBool("isAttacking", false);
    }

    public void Update()
    {
        if (tower.currentTarget == null)
        {
            tower.ChangeState(new IdleState(tower));
            return;
        }

        // 사거리 체크
        Vector3Int enemyTile = tower.tilemap.WorldToCell(tower.currentTarget.transform.position);
        int dx = Mathf.Abs(tower.towerTile.x - enemyTile.x);
        int dy = Mathf.Abs(tower.towerTile.y - enemyTile.y);
        int distance = Mathf.Max(dx, dy);

        if (distance <= tower.attackRange)
        {
            tower.animator.SetTrigger("AttackTrigger");
            tower.ChangeState(new AttackingState(tower));
        }
    }

    public void Exit()
    {
        tower.animator.SetBool("isAttackReady", false);
    }
}
