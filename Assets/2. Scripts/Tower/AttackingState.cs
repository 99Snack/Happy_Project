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

        int enemyX = Mathf.FloorToInt(tower.currentTarget.transform.position.x);
        int enemyY = Mathf.FloorToInt(tower.currentTarget.transform.position.z);

        Vector2Int enemyTile = new Vector2Int(enemyX, enemyY);

        int dx = Mathf.Abs(tower.Coord.x - enemyTile.x);
        int dy = Mathf.Abs(tower.Coord.y - enemyTile.y);
        int distance = Mathf.Max(dx, dy);

        // 사거리 체크
        int enemyX = Mathf.FloorToInt(tower.currentTarget.transform.position.x);
        int enemyY = Mathf.FloorToInt(tower.currentTarget.transform.position.z);

        Vector2Int enemyTile = new Vector2Int(enemyX, enemyY);

        int dx = Mathf.Abs(tower.Coord.x - enemyTile.x);
        int dy = Mathf.Abs(tower.Coord.y - enemyTile.y);
        int distance = Mathf.Max(dx, dy);

        //사거리 벗어나면
        if (distance > tower.attackRange)
        {
            tower.currentTarget = null;
            tower.ChangeState(new AttackStopState(tower));
            return;
        }
        // 사거리 안에 있으면
        else
        {
            if (tower.attackCooldown <= 0f)
            {
                tower.shooter.Shoot(tower.currentTarget, 0f, tower.attackHitCount);
                tower.attackCooldown = 1f / tower.attackSpeed;
            }
        }
    }

    public void Exit()
    {
        tower.animator.SetBool("isAttacking", false);
    }
}
