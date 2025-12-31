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

    // --- [추가된 부분: 별 표시 기능] ---
    [Header("Grade Visuals")]
    public GameObject[] stars; // 인스펙터에서 별 3개를 연결하세요.

    public void UpdateGradeVisual()
    {
        if (stars == null || stars.Length == 0) return;

        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] != null)
            {
                // 데이터의 Grade(1, 2, 3)에 따라 별을 켭니다.
                stars[i].SetActive(i < Data.Grade);
            }
        }
    }
    // --------------------------------

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

    public float AttackClipLength { get; private set; } = 1.0f;

    public void Setup(int towerId, TileInteractor tile)
    {
        MyTile = tile;
        Data = DataManager.Instance.TowerBaseData[towerId];

        SetAttackClipLength();
        ResetCooldown(data.AttackInterval);
        ResetStatus();

        // [추가] 초기 생성 시 별 표시
        UpdateGradeVisual();

        SetState(this);
        ChangeState(IdleState);
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

        if (currentState == null && MyTile != null)
        {
            SetState(this);
            ChangeState(IdleState);
        }
    }

    private void FixedUpdate()
    {
        if (MyTile == null || MyTile.Type == TileInfo.TYPE.Wait) return;

        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.fixedDeltaTime;
        }

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
        if (MyTile == null || MyTile.Type == TileInfo.TYPE.Wait) return;
        currentState?.Update();
    }

    public void ChangeState(ITowerState newState)
    {
        if (newState == null) return;
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

        // [추가] 승급 시 별 표시 갱신
        UpdateGradeVisual();

        ResetStatus();
    }

    public void OnSold()
    {
        GameManager.Instance.Gold += Data.price;
        Destroy(gameObject);
    }

    public bool IsTargetInRange()
    {
        if (currentTarget == null) return false;

        int enemyX = Mathf.RoundToInt(currentTarget.transform.position.x);
        int enemyY = Mathf.RoundToInt(currentTarget.transform.position.z);

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
                if (augment.Tag == 1) GameManager.Instance.MeleeBonusGold = augment.Value_N;
                else if (augment.Tag == 2) GameManager.Instance.RangeBonusGold = augment.Value_N;
                break;
        }
    }

    void ResetStatus()
    {
        atkPower.baseStat = CalcAttackOfficial();
        atkPower.additiveStat = 0;
        atkPower.multiStat = 1;

        AugmentManager.Instance.ApplyAllActiveAugmentsToTower(this);
    }

    public virtual void Attack()
    {
        animator.SetTrigger(hashAttack);
    }

    public virtual void ExecuteDamage()
    {
        if (currentTarget == null) return;

        if (onHitAugs.Count > 0)
        {
            foreach (var aug in onHitAugs) aug.OnHit(this, currentTarget);
        }
        else
        {
            currentTarget.TakeDamage(atkPower.finalStat, this);
        }
    }

    public virtual int CalcAttackOfficial() => 1;

    public int CalcStageStat(AugmentData augment) =>
        Mathf.FloorToInt((augment.Value_N + augment.CalcGrowValue()));

    protected void SetAttackClipLength()
    {
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name.ToLower().Contains("attack"))
                {
                    AttackClipLength = clip.length;
                    return;
                }
            }
        }
    }
}