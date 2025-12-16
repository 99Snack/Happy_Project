using UnityEngine;

public class AttackingState : ITowerState
{
    private Tower tower;

    public AttackingState(Tower tower)
    {
        this.tower = tower;
    }

    public void Enter()
    {
        Debug.Log("[타워] Attacking 상태 진입");
    }

    public void Update()
    {
        if (tower.currentTarget == null)
        {
            Debug.Log("[타워] 타겟 소실 → 탐색 복귀");
            tower.ChangeState(new SearchingState(tower));
            return;
        }

        Vector3Int enemyTile =
            tower.tilemap.WorldToCell(tower.currentTarget.transform.position);

        int dx = Mathf.Abs(tower.towerTile.x - enemyTile.x);
        int dy = Mathf.Abs(tower.towerTile.y - enemyTile.y);
        int distance = Mathf.Max(dx, dy);

        Debug.Log(
            $"[타워] 공격 유지 검사 | 타워({tower.towerTile.x},{tower.towerTile.y}) " +
            $"적({enemyTile.x},{enemyTile.y}) 거리={distance} / 사거리={tower.attackRange}"
        );

        if (distance > tower.attackRange)
        {
            Debug.Log("[타워] 사거리 이탈 → 타겟 해제");
            tower.currentTarget = null;
            tower.ChangeState(new SearchingState(tower));
            return;
        }

        if (tower.attackCooldown <= 0f)
        {
            Debug.Log("[타워] 총알 발사!");
            tower.shooter.Shoot(
                tower.currentTarget,
                0f,
                tower.attackHitCount
            );

            tower.attackCooldown = 1f / tower.attackSpeed;
        }
    }

    public void Exit() { }
}
