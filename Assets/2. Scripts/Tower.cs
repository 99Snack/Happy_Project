using UnityEngine;

public class Tower : MonoBehaviour
{
    public TowerTargetDetector targetDetector; // 적 탐지
    public TowerShooter shooter;               // 공격
    public Transform baseCamp;                 // 기준 BaseCamp

    private enum TowerState { Idle, Searching, Attacking }
    private TowerState currentState = TowerState.Idle;

    private Enemy currentTarget;

    public float detectionInterval = 0.1f; // 탐지 간격
    private float detectionTimer = 0f;

    public float attackPower = 10f;   // 공격력 (현재 사용 X)
    public float attackSpeed = 1f;    // 초당 공격 횟수
    public float attackRange = 5f;    // BaseCamp 기준 공격 범위
    public int attackHitCount = 1;    // 한 번에 타격 가능한 적 수
    private float attackCooldown = 0f;

    void Update()
    {
        // 상태 처리
        switch (currentState)
        {
            case TowerState.Idle:
                currentState = TowerState.Searching; // Idle → Searching
                break;
            case TowerState.Searching:
                HandleSearching();
                break;
            case TowerState.Attacking:
                HandleAttacking();
                break;
        }

        // 쿨타임 감소
        if (attackCooldown > 0f)
            attackCooldown -= Time.deltaTime;
    }

    // 탐지 처리
    void HandleSearching()
    {
        detectionTimer -= Time.deltaTime;
        if (detectionTimer <= 0f)
        {
            // BaseCamp 기준 가장 가까운 적 선택
            currentTarget = targetDetector.FindNearestEnemy(baseCamp.position);
            detectionTimer = detectionInterval;

            if (currentTarget != null)
            {
                float distanceToBase = Vector3.Distance(baseCamp.position, currentTarget.transform.position);
                if (distanceToBase <= attackRange)
                    currentState = TowerState.Attacking; // 공격 상태 전환
            }
        }
    }

    // 공격 처리
    void HandleAttacking()
    {
        if (currentTarget == null)
        {
            currentState = TowerState.Searching; // 타겟 없으면 탐색
            return;
        }

        // BaseCamp 기준 범위 체크
        float distanceToBase = Vector3.Distance(baseCamp.position, currentTarget.transform.position);
        if (distanceToBase > attackRange)
        {
            currentTarget = null;
            currentState = TowerState.Searching;
            return;
        }

        // 공격 쿨타임 체크
        if (attackCooldown <= 0f)
        {
            Debug.Log("Shooting at: " + currentTarget.name);
            shooter.Shoot(currentTarget, attackPower, attackHitCount);
            attackCooldown = 1f / attackSpeed;
        }
    }
}
