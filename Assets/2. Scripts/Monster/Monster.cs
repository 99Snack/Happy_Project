using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 몬스터 게임 로직을 관리하는 클래스 
/// </summary>
public class Monster : MonoBehaviour
{
    //몬스터 스탯 데이터 
    [SerializeField] private MonsterData data;
    public MonsterData Data { get => data; set => data = value; }
    [SerializeField] private Slider hpBar;

    public Stat defense;
    public Stat moveSpeed;
    public float attackCooldown;

    //애니메이션 관련
    private Animator anim;
    private static readonly int hashHit = Animator.StringToHash("Hit");
    private static readonly int hashTurnRight = Animator.StringToHash("TurnRight");
    private static readonly int hashAttack = Animator.StringToHash("Attack");
    private static readonly int hashTurnLeft = Animator.StringToHash("TurnLeft");
    private static readonly int hashDie = Animator.StringToHash("Die");
    private static readonly int hashSpawn = Animator.StringToHash("Spawn");

    //디버프 관련
    Dictionary<int, (DebuffData, Coroutine)> activeDebuffs = new Dictionary<int, (DebuffData, Coroutine)>();

    public int currentHp; // 현재 체력 (DB에 없어서 여기에 선언)
    private Slider hpSlider; // 몹 hp ui 

    // 스폰 번호(순서) 길찾기에서 사용용
    public int SpawnNumber { get; private set; }

    private bool isDead = false; // 몬스터 사망 여부 // 중복 사망 방지용 

    private void Awake()
    {
        anim = GetComponent<Animator>();  // 애니메이터 컴포넌트 가져오기
        if (anim == null)
        {
            anim = transform.GetComponentInChildren<Animator>(); // 애니메이터 컴포넌트가 자식에 있을때
        }
    }

    private void FixedUpdate()
    {
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.fixedDeltaTime;
        }

        Vector3 allyBasePos = TileManager.Instance.GetWorldPosition(TileManager.Instance.allyBasePosition);
        float distance = Vector3.Distance(transform.position, allyBasePos);
        if (distance < 1.5)
        {
            if (attackCooldown <= 0)
            {
                Attack();
                BaseCamp.Instance.TakeDamage(data.Atk);
                attackCooldown = Data.AtkInterval_ms;
            }
        }
    }

    void UpdateHpUI()
    {
        if (hpSlider != null)
        {
            //int maxHp = (Data != null) ? currentHp : 100;
            float current = (float)currentHp / Data.Hp;
            hpSlider.DOValue(current, 0.25f).SetEase(Ease.Linear);
        }
    }

    public void TakeDamage(float damage, Tower attacker)
    {
        if (isDead) return; // 이미 죽은 몬스터면 무시

        OnHit();

        currentHp -= Mathf.FloorToInt(damage * (1 - defense.finalStat));
        //Debug.Log(gameObject.name + "남은 체력: " + currentHp);
        UpdateHpUI();
        if (currentHp <= 0)
        {
            Die(attacker);

        }
    }

    public void TakeDebuff(DebuffData debuff = null)
    {
        if (debuff == null) return;

        int key = debuff.DebuffId;

        if (activeDebuffs.TryGetValue(key, out var value))
        {
            if (value.Item2 != null) StopCoroutine(value.Item2);
        }

        Coroutine newCoroutine = StartCoroutine(DebuffRoutine(debuff));
        activeDebuffs[key] = (debuff, newCoroutine);
    }

    IEnumerator DebuffRoutine(DebuffData debuff)
    {
        CalcUpdateStat(debuff);

        yield return new WaitForSeconds(debuff.Duration);

        activeDebuffs.Remove(debuff.DebuffId);

        CalcUpdateStat(debuff);
    }

    void CalcUpdateStat(DebuffData debuff)
    {
        //Debug.Log($"{activeDebuffs.Count} : {debuff.DebuffId} : {debuff.DebuffPower}");
        float finalValue = 1;

        float maxValue = 0;
        foreach (var tuple in activeDebuffs.Values)
        {
            if (tuple.Item1.Type == debuff.Type)
            {
                if (tuple.Item1.DebuffPower > maxValue) maxValue = tuple.Item1.DebuffPower;
            }
        }

        finalValue = 1 - maxValue;

        switch (debuff.Type)
        {
            case 1:
                moveSpeed.multiStat = finalValue;
                break;
            case 2:
                defense.multiStat = finalValue;
                break;
        }

        //Debug.Log($"{finalValue} : {debuff.DebuffId}");
    }

    void ResetStatus()
    {
        currentHp = data.Hp;
        isDead = false;

        moveSpeed.baseStat = data.MoveSpeed;
        moveSpeed.additiveStat = 0;
        moveSpeed.multiStat = 1;

        defense.baseStat = data.Defense;
        defense.additiveStat = 0;
        defense.multiStat = 1;

        attackCooldown = data.AtkInterval_ms;

        UpdateHpUI();
    }

    // 스포너에서 호출해서 번호 설정
    public void SetSpawnNumber(MonsterData data, int number)
    {
        SpawnNumber = number;

        //데이터 설정해주기
        Data = data;
        ResetStatus();

        TowerTargetDetector.Instance.RegisterEnemy(this);
    }

    // 길찾기에서 몬스터의 번호 받아가는 용도 
    public int GetSpawnNumber()
    {
        return SpawnNumber;
    }

    // 몬스터 죽을 때 호출 
    //처형 증강 있을 때 이거 불러오기
    public void Die(Tower attacker)
    {
        if (isDead) return; // 중복 사망 방지 
        isDead = true; // 사망 처리
        currentHp = 0;

        GiveReward(attacker);

        // 스폰 매니저에 알리기
        SpawnManager spawnManager = FindAnyObjectByType<SpawnManager>();
        if (spawnManager != null)
        {
            spawnManager.OnMonsterDie(this);
        }

        string monsterId = data.MonsterId.ToString();
        ObjectPoolManager.Instance.ReturnToPool(monsterId, gameObject);
        TowerTargetDetector.Instance.UnregisterEnemy(this);
        //Destroy(gameObject);
    }

    void GiveReward(Tower attacker)
    {
        if (Data == null || Data.RewardGroup <= 0) return;

        if (DataManager.Instance.RewardGroupData.TryGetValue(Data.RewardGroup, out List<RewardGroupData> rewardList))
        {
            foreach (var groupData in rewardList)
            {
                if (DataManager.Instance.RewardData.TryGetValue(groupData.RewardId, out RewardData reward))
                {
                    switch (reward.RewardType)
                    {
                        case 1:
                            GameManager.Instance.Gold += groupData.RewardCount;
                            //Debug.Log($"{groupData.RewardCount} 골드 획득! (현재: {GameManager.Instance.Gold})");
                            break;
                    }
                }
            }
        }
        if (attacker != null)
        {
            float bonusGold =
            attacker.Data.MainType == 1 ?
            GameManager.Instance.MeleeBonusGold : GameManager.Instance.RangeBonusGold;

            if (bonusGold > 0)
            {
                GameManager.Instance.Gold += Mathf.FloorToInt(bonusGold);
            }
        }
    }

    #region Animation
    public void OnHit()
    {
        if (anim != null) anim.SetTrigger(hashHit);   // 피격 애니메이션 다 출력되면 다시 이동으로 바뀜. 다른 메서드도 동일 
        // 타워 공격을 맞을 때 피격과 상호작용
    }

    public void Attack()
    {
        if (anim != null) anim.SetTrigger(hashAttack);

        // 아마 여기서 베이스캠프를 공격하면 피해를 입히게 할 듯
    }
    public void Dead()
    {
        if (anim != null) anim.SetTrigger(hashDie);

    }

    public void TurnLeft()
    {
        if (anim != null) anim.SetTrigger(hashTurnLeft);

    }

    public void TurnRight()
    {
        if (anim != null) anim.SetTrigger(hashTurnRight);
    }

    public void Spawn()
    {
        //스폰하는동안 이동못하게 코루틴 필요
        if (anim != null) anim.SetTrigger(hashSpawn);
    }
    #endregion
}