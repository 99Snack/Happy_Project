using UnityEngine;
using UnityEngine.EventSystems;

public enum TowerSlot
{
    None, Wait, Tile
}

public abstract class Tower : MonoBehaviour, IPointerClickHandler
{
    public TowerShooter shooter;
    public Animator animator;

    public TileInteractor MyTile{ get; private set; }
    private TowerSlot towerSlot = TowerSlot.None;

    public Tower_Base Data { get; private set; }
    // 타워 좌표
    public Vector2Int Coord { get; set; }
    public TowerSlot TowerSlot { get => towerSlot; set => towerSlot = value; }

    // 현재 타겟
    [HideInInspector] public MonsterMove currentTarget;

    [HideInInspector] public float attackCooldown = 0f;

    // 상태 패턴 FSM
    private ITowerState currentState;
    protected IdleState IdleState;
    protected AttackStopState AttackStopState;

    public void Setup(int towerId, TileInteractor tile)
    {
        towerSlot = TowerSlot.Wait;
        //데이터매니저에서 데이터 가져오기
        Data = DataManager.Instance.TowerBaseData[towerId];

        MyTile = tile;
    }

    public void SetMyTile(TileInteractor tile){
        MyTile = tile;
    }

    public void SetCoord(int x, int y){
        Coord = new Vector2Int(x, y);
    }

    protected virtual void Start()
    {
        //towerTile = tilemap.WorldToCell(transform.position);
        //Debug.Log($"[타워] 타워 타일 좌표: ({towerTile.x},{towerTile.y})");

        ChangeState(new IdleState(this));
    }

    protected virtual void Update()
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

    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.Instance.OpenTowerInfo(this);
    }

    public void Upgrade()
    {
        //1. 타워가 합쳐지고
        //2. Tower_base data가 업그레이드 된 등급으로 변경
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
        if (currentTarget == null)
            return false;


        int enemyX = Mathf.FloorToInt(currentTarget.transform.position.x);
        int enemyY = Mathf.FloorToInt(currentTarget.transform.position.z);

        int dx = Mathf.Abs(enemyX - Coord.x);
        int dy = Mathf.Abs(enemyY - Coord.y);


        return Mathf.Max(dx, dy) <= Data.Range;
    }

    public abstract void Attack();
}


