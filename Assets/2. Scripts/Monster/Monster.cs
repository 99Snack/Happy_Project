using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
/// <summary>
/// 몬스터 게임 로직을 관리하는 클래스 
/// </summary>
public class Monster : MonoBehaviour
{
    //몬스터 스탯 데이터 
    public MonsterData Data;
    public float FinalDefence { get; set; }


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
            anim = transform.GetChild(0).GetComponent<Animator>(); // 애니메이터 컴포넌트가 자식에 있을때
        }
    }

 
   
    void Start()
    {
        currentHp = (Data != null) ? Data.Hp : 100; // 시작 시 현재 체력 = 최대 체력 
        
        // 몬스터 HP UI 
        GameObject hpBarPrefab = Resources.Load<GameObject>("Prefab/Monster/MonsterHpBar");
        // 디버그 확인
        if (hpBarPrefab == null)
        {
            Debug.LogError("HP바 프리팹을 찾을 수 없음! 경로 확인 필요");
        }
       else 
        {
            GameObject hpBar = Instantiate(hpBarPrefab, transform);
            hpBar.transform.localPosition = new Vector3(0, 2.5f, 0); // 몬스터 머리 위
           //hpBar.transform.localScale = new Vector3(1, 1f, 1);
            hpSlider = hpBar.GetComponentInChildren<Slider>();
            Debug.Log("HP바 생성 완료");
        }
        UpdateHpUI(); 

        TowerTargetDetector.Instance.RegisterEnemy(this);
    }

    void UpdateHpUI()
    {
        if (hpSlider != null)
        {
            int maxHp = (Data != null) ? Data.Hp : 100;
            hpSlider.value = (float)currentHp / maxHp;
        }
    }
    // 총알 충돌 (총알 프리팹에 태그 불렛과, 콜라이더에 Is Trigger 체크 필요)
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet")) // 총알 태그와 충돌했을 때 // 
        {
            OnHit(); // 피격 애니메이션 호출 

            Projectile projectile = other.GetComponent<Projectile>();
            if (projectile != null)
            {
                TakeDamage(projectile.damage); // 피 까임
            }

            //  Destroy(other.gameObject); // 총알 파괴 // 현재 프로젝타일에서 자체 파괴 처리함
        }

        //if (other.CompareTag("BaseCamp")) // 베이스캠프 태그와 충돌했을 때
        //{
        //    Attack();
        //}
        // 나중에 hp도 까이게 // 이건 몬스터에서 처리함 
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // 이미 죽은 몬스터면 무시

        OnHit();

        currentHp -= damage;
        Debug.Log(gameObject.name + "남은 체력: " + currentHp);
        UpdateHpUI();
        if (currentHp <= 0)
        {
            Die();

        }
    }

    public void TakeDebuff(DebuffData debuff = null)
    {
        if (debuff == null) return;

        int key = debuff.DebuffId;
        if (activeDebuffs.TryGetValue(key, out (DebuffData debuffData, Coroutine coroutine) value))
        {
            if (value.coroutine != null)
            {
                StopCoroutine(value.coroutine);
            }
        }

        Coroutine newCoroutine = StartCoroutine(DebuffRoutine(debuff));
        activeDebuffs[key] = (debuff, newCoroutine);
    }

    IEnumerator DebuffRoutine(DebuffData debuff)
    {
        CalcUpdateStat(debuff);

        yield return new WaitForSeconds(debuff.Duration);

        CalcUpdateStat(debuff);
    }

    void CalcUpdateStat(DebuffData debuff)
    {
        int finalValue = 1;

        foreach (var tuple in activeDebuffs.Values)
        {
            finalValue = Mathf.FloorToInt(1 - tuple.Item1.DebuffPower);
        }

        switch (debuff.Type)
        {
            case 1:
                //FinalMoveSpeed = Data.MoveSpeed * finalValue;
                break;
            case 2:
                FinalDefence = Data.Defense * finalValue;
                break;
        }
    }


    // 스포너에서 호출해서 번호 설정
    public void SetSpawnNumber(int number)
    {
        SpawnNumber = number;

        //데이터 설정해주기
        //Data = DataManager.Instance.MonsterData[]

        TowerTargetDetector.Instance.RegisterEnemy(this);
    }

    // 길찾기에서 몬스터의 번호 받아가는 용도 
    public int GetSpawnNumber()
    {
        return SpawnNumber;
    }

    // 몬스터 죽을 때 호출 
    public void Die()
    {
        if (isDead) return; // 중복 사망 방지 
        isDead = true; // 사망 처리


        Debug.Log(gameObject.name + " 몬스터 사망!");

        GiveReward(); // 미션을 잘 완수 했으니 리워드를 받아야겠지? 

        // 스폰 매니저에 알리기
        SpawnManager spawnManager = FindAnyObjectByType<SpawnManager>();
        if (spawnManager != null)
        {
            spawnManager.OnMonsterDie();
        }


        Destroy(gameObject);
    }

    void GiveReward()
    {
        // 데이터가 없거나 리워드 그룹도 비어있으면 리턴
        if (Data == null) return; 
        if (string.IsNullOrEmpty(Data.RewardGroup)) return;

        // 리워드 그룹 데이터 순회(RewardGroupData에서 해당 리워드 그룹 찾기) 
        foreach (var pair in DataManager.Instance.RewardGroupData)
        {
            RewardGroupData groupData = pair.Value;

            // 몬스터의 리워드 그룹이 같은 것만 처리 
            if (groupData.RewardGroup  == Data.RewardGroup)
            { 
                // RewardData에서 리워드 타입 확인
                if (DataManager.Instance.RewardData.TryGetValue(groupData.RewardId, out RewardData reward))
                 {                       
                    if (reward.RewardType ==1) //골드 지급 
                    {
                        GameManager.Instance.Gold += groupData.RewardCount;
                        Debug.Log(groupData.RewardCount + "골드 획득, 총 골드 : " + GameManager.Instance.Gold);

                    }

                }
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