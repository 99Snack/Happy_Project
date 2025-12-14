using UnityEngine;
using UnityEngine.Tilemaps;

public class Tower : MonoBehaviour
{
    // 외부 연결
    public TowerTargetDetector targetDetector;
    public TowerShooter shooter;
    public Transform baseCamp;
    public Tilemap tilemap;

    // 타워 상태 FSM
    private enum State
    {
        Idle,
        Searching,
        Attacking
    }

    private State currentState = State.Idle;

    // 현재 타겟
    private Enemy currentTarget;

    // 탐지 타이머
    public float detectionInterval = 0.1f;
    private float detectionTimer = 0f;

    // 공격 설정
    public float attackSpeed = 1f;   // 초당 공격 횟수
    public int attackRange = 5;      // 타일 기준 공격 범위
    public int attackHitCount = 1;
    private float attackCooldown = 0f;

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

        // 공격 쿨타임 감소
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    // 적 탐색 단계
    void HandleSearching()
    {
        detectionTimer -= Time.deltaTime;
        if (detectionTimer > 0f) return;

        // BaseCamp 위치를 타일 좌표로 변환
        Vector3Int baseCampTile =
            tilemap.WorldToCell(baseCamp.position);

        // BaseCamp 기준 가장 가까운 적 탐색
        currentTarget =
            targetDetector.FindNearestEnemy(baseCampTile);

        detectionTimer = detectionInterval;

        // 적이 존재하면 공격 상태로 전환
        if (currentTarget != null)
        {
            Vector3Int enemyTile =
                tilemap.WorldToCell(currentTarget.transform.position);

            int tileDistance =
                Mathf.Abs(baseCampTile.x - enemyTile.x) +
                Mathf.Abs(baseCampTile.z - enemyTile.z);

            if (tileDistance <= attackRange)
            {
                currentState = State.Attacking;
            }
        }
    }

    // 공격 단계
    void HandleAttacking()
    {
        // BaseCamp 타일 좌표
        Vector3Int baseCampTile =
            tilemap.WorldToCell(baseCamp.position);

        // 현재 가장 가까운 적을 다시 계산
        Enemy nearest =
            targetDetector.FindNearestEnemy(baseCampTile);

        // 더 가까운 적이 있다면 타겟 교체
        if (nearest != null && nearest != currentTarget)
        {
            currentTarget = nearest;
        }

        if (currentTarget == null)
        {
            currentState = State.Searching;
            return;
        }

        // 타일 거리 계산
        Vector3Int enemyTile =
            tilemap.WorldToCell(currentTarget.transform.position);

        int tileDistance =
            Mathf.Abs(baseCampTile.x - enemyTile.x) +
            Mathf.Abs(baseCampTile.z - enemyTile.z);

        // 공격 범위 벗어나면 탐색으로 복귀
        if (tileDistance > attackRange)
        {
            currentTarget = null;
            currentState = State.Searching;
            return;
        }

        // 발사
        if (attackCooldown <= 0f)
        {
            shooter.Shoot(currentTarget, 0f, attackHitCount);
            attackCooldown = 1f / attackSpeed;
        }
    }
}
