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
        Debug.Log("[타워] Searching 상태 진입");
    }

    public void Update()
    {
        tower.detectionTimer -= Time.deltaTime;
        if (tower.detectionTimer > 0f) return;

        Vector3Int baseCampTile =
            tower.tilemap.WorldToCell(tower.baseCamp.position);

        tower.currentTarget =
            tower.targetDetector.FindNearestEnemyInRange(
                tower.towerTile,
                tower.attackRange,
                baseCampTile
            );

        tower.detectionTimer = tower.detectionInterval;

        Debug.Log(
            $"[타워] 탐색 결과: {(tower.currentTarget != null ? "타겟 선정 완료" : "사거리 내 적 없음")}"
        );

        if (tower.currentTarget != null)
        {
            Debug.Log("[타워] 공격 상태로 전환");
            tower.ChangeState(new AttackingState(tower));
        }
    }

    public void Exit() { }
}
