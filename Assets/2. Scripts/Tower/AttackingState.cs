using UnityEngine;
using UnityEngine.Tilemaps;

public class AttackingState : ITowerState
{
    private Tower tower;

    public AttackingState(Tower tower)
    {
        this.tower = tower;
    }

    public void Enter()
    {
        Debug.Log("[타워] Attack 상태 진입");
        tower.animator.SetBool("isAttacking", true);
        tower.animator.SetTrigger("AttackTrigger");
    }

    public void Update()
    {
        if (tower.currentTarget == null)
        {
            tower.ChangeState(new AttackStopState(tower));
            return;
        }

        Vector3Int enemyTile = tower.tilemap.WorldToCell(tower.currentTarget.transform.position);
        int dx = Mathf.Abs(tower.towerTile.x - enemyTile.x);
        int dy = Mathf.Abs(tower.towerTile.y - enemyTile.y);
        int distance = Mathf.Max(dx, dy);

        if (distance > tower.attackRange)
        {
            tower.currentTarget = null;
            tower.ChangeState(new AttackStopState(tower));
            return;
        }

        if (tower.attackCooldown <= 0f)
        {
            tower.shooter.Shoot(tower.currentTarget, 0f, tower.attackHitCount);
            tower.attackCooldown = 1f / tower.attackSpeed;
        }
    }

    public void Exit()
    {
        tower.animator.SetBool("isAttacking", false);
    }
}
