using UnityEngine;
using UnityEngine.Tilemaps;

public class Tower : MonoBehaviour
{
    public TowerTargetDetector targetDetector;
    public TowerShooter shooter;
    public Transform baseCamp;
    public Tilemap tilemap;

    // 타워 타일 좌표
    private Vector3Int towerTile;

    // FSM
    private enum State
    {
        Idle,
        Searching,
        Attacking
    }

    private State currentState = State.Idle;
    private Enemy currentTarget;

    // 탐지
    public float detectionInterval = 0.1f;
    private float detectionTimer = 0f;

    // 공격
    public float attackSpeed = 1f;
    public int attackRange = 1;
    public int attackHitCount = 1;
    private float attackCooldown = 0f;

    void Start()
    {
        towerTile = tilemap.WorldToCell(transform.position);
        Debug.Log($"[타워] 타워 타일 좌표: ({towerTile.x},{towerTile.y})");
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                currentState = State.Searching;
                break;

            case State.Searching:
                HandleSearching();
                break;

            case State.Attacking:
                HandleAttacking();
                break;
        }

        if (attackCooldown > 0f)
            attackCooldown -= Time.deltaTime;
    }

    // 탐색 단계 
    void HandleSearching()
    {
        detectionTimer -= Time.deltaTime;
        if (detectionTimer > 0f) return;

        Vector3Int baseCampTile = tilemap.WorldToCell(baseCamp.position);

        currentTarget = targetDetector.FindNearestEnemyInRange(
            towerTile,
            attackRange,
            baseCampTile
        );

        detectionTimer = detectionInterval;

        Debug.Log(
            $"[타워] 탐색 결과: {(currentTarget != null ? "타겟 선정 완료" : "사거리 내 적 없음")}"
        );

        if (currentTarget != null)
        {
            Debug.Log("[타워] 공격 상태로 전환 (타겟 고정)");
            currentState = State.Attacking;
        }
    }

    // 공격 단계
    void HandleAttacking()
    {
        if (currentTarget == null)
        {
            Debug.Log("[타워] 타겟 소실 → 탐색 복귀");
            currentState = State.Searching;
            return;
        }

        // 사거리 유지 체크
        Vector3Int enemyTile = tilemap.WorldToCell(currentTarget.transform.position);

        int dx = Mathf.Abs(towerTile.x - enemyTile.x);
        int dy = Mathf.Abs(towerTile.y - enemyTile.y);
        int distance = Mathf.Max(dx, dy);

        Debug.Log(
            $"[타워] 공격 유지 검사 | 타워({towerTile.x},{towerTile.y}) " +
            $"적({enemyTile.x},{enemyTile.y}) 거리={distance} / 사거리={attackRange}"
        );

        if (distance > attackRange)
        {
            Debug.Log("[타워] 사거리 이탈 → 타겟 해제");
            currentTarget = null;
            currentState = State.Searching;
            return;
        }

        // 공격
        if (attackCooldown <= 0f)
        {
            Debug.Log("[타워] 총알 발사!");
            shooter.Shoot(currentTarget, 0f, attackHitCount);
            attackCooldown = 1f / attackSpeed;
        }
    }
}
