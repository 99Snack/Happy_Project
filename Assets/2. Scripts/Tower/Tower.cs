using UnityEngine;

public class Tower : MonoBehaviour
{
    public TowerShooter shooter;
    public Animator animator;

    // 타워 좌표
    public Vector2Int Coord  { get; set; }

    // 현재 타겟
    [HideInInspector] public MonsterMove currentTarget;

    // 탐지
    public float detectionInterval = 0.1f;
    [HideInInspector] public float detectionTimer = 0f;

    // 공격
    public float attackSpeed = 1f;
    public int attackRange = 1;
    public int attackHitCount = 1;
    [HideInInspector] public float attackCooldown = 0f;

    // 상태 패턴 FSM
    private ITowerState currentState;

    public void Setup(int x ,int y)
    {
        Coord = new Vector2Int(x, y);
    }

    void Start()
    {
        //towerTile = tilemap.WorldToCell(transform.position);
        //Debug.Log($"[타워] 타워 타일 좌표: ({towerTile.x},{towerTile.y})");

        ChangeState(new IdleState(this));
    }

    void Update()
    {
        currentState?.Update();

        if (attackCooldown > 0f)
            attackCooldown -= Time.deltaTime;
    }

    public void ChangeState(ITowerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void OnAttackStopEnd()
    {
        //if (currentTarget != null)
        //{
        //    ChangeState(new SearchingState(this)); // AttackReady 역할
        //}
        //else
        //{
        //    ChangeState(new IdleState(this));
        //}
    }
}


