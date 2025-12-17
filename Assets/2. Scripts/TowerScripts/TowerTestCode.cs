using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerTestCode : MonoBehaviour
{
    public TowerShooter shooter;
    public Animator animator;
    int x;
    int y;

    public TowerTargetDetector ttd;
    //// 타워 타일 좌표
    //[HideInInspector] public Vector3Int towerTile;

    //// 현재 타겟
    //[HideInInspector] public Enemy currentTarget;

    //// 탐지
    //public float detectionInterval = 0.1f;
    //[HideInInspector] public float detectionTimer = 0f;

    //// 공격
    //public float attackSpeed = 1f;
    //public int attackRange = 1;
    //public int attackHitCount = 1;
    [HideInInspector] public float attackCooldown = 0f;

    // 상태 패턴 FSM
    private ITowerState currentState;

    public void Setup(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    void Awake()
    {
        if (animator == null)
        {
            TryGetComponent(out animator);
        }

        if (ttd == null)
        {
            TryGetComponent(out ttd);
        }
    }

    void Update()
    {
        currentState?.Update();

        if (attackCooldown > 0f)
            attackCooldown -= Time.deltaTime;

        //적이 위치한 타일이 타워 사거리내에 들어오면 공격애니메이션으로 전환

        //1. 적의 월드 포지션 기준 타일 좌표 가져오기
        Vector2Int allyBaseCoord = TileManager.Instance.allyBasePosition;
        Vector2Int towerCoord = new Vector2Int(x, y);


        //2. 가져온 타일 좌표와 타워 좌표 거리 계산하기
        MoveTest enemy = ttd.FindNearestEnemyInRange(towerCoord, 1, allyBaseCoord);

        //3. 거리가 타워 사거리 내에 있다면 공격
        if (enemy != null)
        {
            animator.SetBool("isAttackReady", true);
            animator.SetTrigger("AttackTrigger");
            shooter.Shoot(enemy, 0f, 1);
        }
    }



    public void ChangeState(ITowerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
