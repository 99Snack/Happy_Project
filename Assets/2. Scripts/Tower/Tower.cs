using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    [SerializeField] private Tower_Base data;

    public TowerShooter shooter;

    public Animator animator;

    // 타워 좌표
    public Vector2Int Coord { get; private set; }
    public Tower_Base Data { get => data; private set => data = value; }

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

    protected virtual void Start()
    {
        Debug.Log("Tower Start");
        //ChangeState(new IdleState(this));
    }

    protected virtual void Update()
    {
        Debug.Log("Update");
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
    public void SetData(Tower_Base data)
    {
        //Data = new Tower_Base(data);
    }

    public void Setup(int x, int y)
    {
        Coord = new Vector2Int(x, y);
    }
    public abstract void Attack();
}