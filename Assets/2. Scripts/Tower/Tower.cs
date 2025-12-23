using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Tower : MonoBehaviour, IPointerClickHandler
{
    public TowerShooter shooter;
    public Animator animator;

    public TileInteractor MyTile { get; private set; }
    public TowerBaseData Data { get; private set; }
    public Vector2Int Coord { get; set; }
    public float PlacedTime { get; set; }

    public bool IsRotate { get; protected set; } //회전체가 있으면 true : false
    public Transform Soldier;

    public LayerMask monsterLayer;

    // 현재 타겟
    [HideInInspector] public Monster currentTarget;
    [HideInInspector] public float attackCooldown = 0f;

    // 상태 패턴 FSM
    private ITowerState currentState;
    protected IdleState IdleState;
    protected AttackStopState AttackStopState;

    public readonly int hashIsReady = Animator.StringToHash("IsReady");
    public readonly int hashAttack = Animator.StringToHash("Attack");
    public readonly int hashIsCooldown = Animator.StringToHash("IsCooldown");
    public readonly int hashAttackInterval = Animator.StringToHash("AttackInterval");
    public readonly int hashIsAttacking = Animator.StringToHash("IsAttacking");

    //코루틴 관련
    Coroutine CoSearch;

    public void Setup(int towerId, TileInteractor tile)
    {
        MyTile = tile;

        //데이터매니저에서 데이터 가져오기
        Data = DataManager.Instance.TowerBaseData[towerId];
        attackCooldown = Data.AttackInterval;
    }

    public void SetMyTile(TileInteractor tile) => MyTile = tile;

    public void SetCoord(int x, int y) => Coord = new Vector2Int(x, y);

    protected virtual void Start()
    {
        //towerTile = tilemap.WorldToCell(transform.position);
        //Debug.Log($"[타워] 타워 타일 좌표: ({towerTile.x},{towerTile.y})");
        if (shooter == null)
        {
            shooter = GetComponent<TowerShooter>();
        }
        //ChangeState(new IdleState(this));
    }

    protected virtual void Update()
    {
        if (MyTile.Type == TileInfo.TYPE.Wait) return;

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

    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.Instance.OpenTowerInfo(this);
    }

    public void Upgrade()
    {
        //1. 타워가 합쳐지고
        //2. Tower_base data가 업그레이드 된 등급으로 변경
        //다음 등급으로 데이터 변경
        Data = DataManager.Instance.TowerBaseData[Data.TowerID + 1];
    }

    public void OnSold()
    {
        //연결된 이펙트나 사운드 제거
        //옵젝 제거하거나 풀링반환
        GameManager.Instance.Gold += Data.price;
        Destroy(gameObject);
    }

    public bool IsTargetInRange()
    {
        if (currentTarget == null) return false;

        int enemyX = Mathf.FloorToInt(currentTarget.transform.position.x);
        int enemyY = Mathf.FloorToInt(currentTarget.transform.position.z);

        Vector2Int enemyTile = new Vector2Int(enemyX, enemyY);

        int dx = Mathf.Abs(enemyX - Coord.x);
        int dy = Mathf.Abs(enemyY - Coord.y);

        return Mathf.Max(dx, dy) <= Data.Range;
    }

    public void SearchingCoroutine(IEnumerator enumerator)
    {
        if (CoSearch != null) return;

        CoSearch = StartCoroutine(enumerator);
    }

    public void SearchingStopCoroutine()
    {
        if (CoSearch != null)
        {
            StopCoroutine(CoSearch);
            CoSearch = null;
        }
    }

    public bool CanAttack() => attackCooldown <= 0f ? true : false;
    public void ResetCooldown(float interval) => attackCooldown = interval;

    public virtual void Attack()
    {
        if (currentTarget == null) return;

        int attackPower = CalcAttackPower();

        currentTarget.TakeDamage(attackPower);

        if (CanAttack())
        {
            ResetCooldown(Data.AttackInterval);
            animator.SetTrigger(hashAttack);
        }
    }

    public virtual int CalcAttackPower()
    {
        //1회 공격 피해량 = 타워 공격력 x 타격 수 x(1 – 몬스터 방어력)
        //기본 단일 공격 공식
        //todo : return Data.Attack * Data.HitCount * (1 - monster.Data.Defense);
        return Data.Attack * Data.HitCount;
    }

}


