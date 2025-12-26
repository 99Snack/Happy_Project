using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class Stat
{
    public float baseStat;
    public float additiveStat;
    public float multiStat;

    public float finalStat => (baseStat + additiveStat) * multiStat;
}


public abstract class Tower : MonoBehaviour, IPointerClickHandler
{
    //public TowerShooter shooter;
    public Animator animator;

    public TileInteractor MyTile { get; private set; }

    [SerializeField] private TowerBaseData data;
    public TowerBaseData Data { get => data; private set => data = value; }
    public Vector2Int Coord { get; set; }
    public float PlacedTime { get; set; }

    public bool IsRotate { get; protected set; } //회전체가 있으면 true : false
    public Transform Soldier;

    public LayerMask monsterLayer;

    // 현재 타겟
    public Monster currentTarget;
    [HideInInspector] public float attackCooldown = 0f;

    //최종 스탯
    public Stat atkPower = new Stat();

    // 상태 패턴 FSM
    private ITowerState currentState;
    public IdleState IdleState;
    public AttackStopState AttackStopState;
    public AttackingState AttackingState;
    public SearchingState SearchingState;

    protected void SetState(Tower tower)
    {
        IdleState = new IdleState(tower);
        AttackStopState = new AttackStopState(tower);
        AttackingState = new AttackingState(tower);
        SearchingState = new SearchingState(tower);
    }

    public readonly int hashIsReady = Animator.StringToHash("IsReady");
    public readonly int hashAttack = Animator.StringToHash("Attack");
    public readonly int hashIsCooldown = Animator.StringToHash("IsCooldown");
    public readonly int hashAttackInterval = Animator.StringToHash("AttackInterval");
    public readonly int hashIsAttacking = Animator.StringToHash("IsAttacking");

    //코루틴 관련
    Coroutine CoSearch;

    //증강 관련
    protected List<IOnHitAugment> onHitAugs = new List<IOnHitAugment>();
    protected List<IOnKillAugment> onKillAugs = new List<IOnKillAugment>();
    protected List<IStatusCheckAugment> onStatusAugs = new List<IStatusCheckAugment>();

    public void Setup(int towerId, TileInteractor tile)
    {
        MyTile = tile;

        //데이터매니저에서 데이터 가져오기
        Data = DataManager.Instance.TowerBaseData[towerId];
        ResetCooldown(data.AttackInterval);
        ResetStatus();
    }

    public void SetMyTile(TileInteractor tile) => MyTile = tile;

    public void SetCoord(int x, int y) => Coord = new Vector2Int(x, y);

    protected virtual void Start()
    {
        if (Soldier != null)
        {
            IsRotate = true;
            animator.applyRootMotion = true;
        }
    }

    private void FixedUpdate()
    {
        if (MyTile.Type == TileInfo.TYPE.Wait) return;

        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.fixedDeltaTime;
        }

        // 상태 체크는 0.5초마다 (최적화)
        statusTimer += Time.fixedDeltaTime;
        if (statusTimer >= 0.5f)
        {
            statusTimer = 0;
            UpdateConditionAugment();
        }
    }

    private float statusTimer = 0f;
    protected virtual void Update()
    {
        if (MyTile.Type == TileInfo.TYPE.Wait) return;

        currentState?.Update();
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
        //todo : 옵젝 제거하거나 풀링반환
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

    public void AddConditionAugment(AugmentData augment)
    {
        object instance = AugmentFactory.CreateInstance(augment);
        Debug.Log(instance);
        if (instance == null) return;

        if (instance is IOnHitAugment hit) onHitAugs.Add(hit);
        if (instance is IOnKillAugment kill) onKillAugs.Add(kill);
        if (instance is IStatusCheckAugment status) onStatusAugs.Add(status);
    }

    protected HashSet<int> appliedConditionAugments = new HashSet<int>();

    public virtual void ApplyAugment(AugmentData augment)
    {
        if (augment.Tag != 0) return;

        // 조건부 증강
        if (augment.Category == 3)
        {
            // 중복 적용 방지
            if (!appliedConditionAugments.Contains(augment.Index))
            {
                AddConditionAugment(augment);
                appliedConditionAugments.Add(augment.Index);
                UpdateConditionAugment();
            }
        }
        else
        {
            // 능력치 증강
            if (augment.Category == 1)
            {
                UpdateStatus(augment);
            }
        }
    }
    public void UpdateConditionAugment()
    {
        foreach (var aug in onStatusAugs)
        {
            aug.UpdateStatus(this);
        }
    }

    // ResetStatus는 타워 초기화나 업그레이드 시에만 호출
    public void ResetAllStatus()
    {
        atkPower.baseStat = CalcAttackOfficial();
        atkPower.additiveStat = 0;
        atkPower.multiStat = 1;

        // 모든 증강 초기화
        onHitAugs.Clear();
        onKillAugs.Clear();
        onStatusAugs.Clear();
        appliedConditionAugments.Clear();
    }

    public void UpdateStatus(AugmentData augment)
    {
        switch (augment.Plus_Factor)
        {
            case 1:
                atkPower.additiveStat += CalcStageStat(augment);
                break;
            case 3:
                if (augment.Tag == 1)
                {
                    GameManager.Instance.MeleeBonusGold = augment.Value_N;
                }
                else if (augment.Tag == 2)
                {
                    GameManager.Instance.RangeBonusGold = augment.Value_N;
                }
                break;
        }
    }

    void ResetStatus()
    {
        atkPower.baseStat = CalcAttackOfficial();
        atkPower.additiveStat = 0;
        atkPower.multiStat = 1;
    }



    public virtual void Attack()
    {
        if (currentTarget == null) return;

        currentTarget.TakeDamage(atkPower.finalStat,this);

        if (CanAttack())
        {
            ResetCooldown(Data.AttackInterval);
            animator.SetTrigger(hashAttack);
        }
    }

    public virtual int CalcAttackOfficial()
    {
        //기본 단일 공격 공식
        //1회 공격 피해량 = 타워 공격력 x 타격 수 x(1 – 몬스터 방어력)

        //기본 디버프 공격 공식
        //1회 공격 피해량 = 타워 공격력 x ( 1 – 몬스터 방어력 ) x 0.8

        //기본 광역 공격 공식
        //1회 공격 피해량 = 타워 공격력 x( 1 – 몬스터 방어력 )

        return 1;
    }

    public int CalcStageStat(AugmentData augment) => Mathf.FloorToInt((augment.Value_N + augment.CalcGrowValue()));

}


