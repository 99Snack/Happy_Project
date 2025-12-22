using UnityEngine;
using UnityEngine.EventSystems;

public enum TowerSlot
{
    None, Wait, Tile
}

public class Tower : MonoBehaviour, IPointerClickHandler
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
}


