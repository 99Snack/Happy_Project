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

[Serializable]
public enum statetest
{
    idle, search, attack, attackstop
}

public abstract class Tower : MonoBehaviour, IPointerClickHandler
{
    public Animator animator;

    public TileInteractor MyTile { get; private set; }

    [SerializeField] private TowerBaseData data;
    public TowerBaseData Data { get => data; private set => data = value; }
    public Vector2Int Coord { get; set; }
    public float PlacedTime { get; set; }

    public bool IsRotate { get; protected set; }
    public Transform Soldier;

    public LayerMask monsterLayer;

    public Monster currentTarget;
    [HideInInspector] public float attackCooldown = 0f;

    public Stat atkPower = new Stat();

    // FSM
    private ITowerState currentState;
    public IdleState IdleState;
    public AttackStopState AttackStopState;
    public AttackingState AttackingState;
    public SearchingState SearchingState;
    public statetest state = statetest.idle;

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

    Coroutine CoSearch;

    protected List<IOnHitAugment> onHitAugs = new List<IOnHitAugment>();
    protected List<IOnKillAugment> onKillAugs = new List<IOnKillAugment>();
    protected List<IStatusCheckAugment> onStatusAugs = new List<IStatusCheckAugment>();

    public void Setup(int towerId, TileInteractor tile)
    {
        MyTile = tile;
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

        // 쿨다운 감소
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.fixedDeltaTime;
        }

        // 상태 체크 최적화
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
        UIManager.Instance.OpenTowerInfoPanel(this);
    }

    public void Upgrade()
    {
        Data = DataManager.Instance.TowerBaseData[Data.TowerID + 1];
    }

    public void OnSold()
    {
        GameManager.Instance.Gold += Data.price;
        Destroy(gameObject);
    }

    public bool IsTargetInRange()
    {
        if (currentTarget == null) return false;

        int enemyX = Mathf.FloorToInt(currentTarget.transform.position.x);
        int enemyY = Mathf.FloorToInt(currentTarget.transform.position.z);

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

    public bool CanAttack() => attackCooldown <= 0f;
    public void ResetCooldown(float interval) => attackCooldown = interval;

    public void AddConditionAugment(AugmentData augment)
    {
        object instance = AugmentFactory.CreateInstance(augment);
        if (instance == null) return;

        if (instance is IOnHitAugment hit) onHitAugs.Add(hit);
        if (instance is IOnKillAugment kill) onKillAugs.Add(kill);
        if (instance is IStatusCheckAugment status) onStatusAugs.Add(status);
    }

    protected HashSet<int> appliedConditionAugments = new HashSet<int>();

    public virtual void ApplyAugment(AugmentData augment)
    {
        if (augment.Tag != 0) return;

        if (augment.Category == 3)
        {
            if (!appliedConditionAugments.Contains(augment.Index))
            {
                AddConditionAugment(augment);
                appliedConditionAugments.Add(augment.Index);
                UpdateConditionAugment();
            }
        }
        else
        {
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

    public void ResetAllStatus()
    {
        atkPower.baseStat = CalcAttackOfficial();
        atkPower.additiveStat = 0;
        atkPower.multiStat = 1;

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

    // FSM에서 호출하는 메서드 (애니메이션 트리거만 발동)
    public virtual void Attack()
    {
        // 이 메서드는 더 이상 직접 데미지를 주지 않음
        // AttackingState에서 애니메이션 트리거만 발동
    }

    // 애니메이션 이벤트에서 호출하는 실제 데미지 처리
    public virtual void ExecuteDamage()
    {
        if (currentTarget == null) return;

        if (onHitAugs.Count > 0)
        {
            foreach (var aug in onHitAugs)
            {
                aug.OnHit(this, currentTarget);
            }
        }
        else
        {
            currentTarget.TakeDamage(atkPower.finalStat, this);
        }
    }

    public virtual int CalcAttackOfficial()
    {
        return 1;
    }

    public int CalcStageStat(AugmentData augment) =>
        Mathf.FloorToInt((augment.Value_N + augment.CalcGrowValue()));
}