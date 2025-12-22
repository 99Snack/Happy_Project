using UnityEngine;

/// <summary>
/// 몬스터 게임 로직을 관리하는 클래스 
/// </summary>
public class Monster : MonoBehaviour
{
    public MonsterData Data; // 몬스터 스탯 데이터  

    public int currentHp; // 현재 체력 (DB에 없어서 여기에 선언)

    // 스폰 번호(순서) 길찾기에서 사용용
    public int SpawnNumber { get; private set; }

    private bool isDead = false; // 몬스터 사망 여부 // 중복 사망 방지용 

    void Start()
    {
        currentHp = Data.Hp; // 시작 시 현재 체력 = 최대 체력 
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // 이미 죽은 몬스터면 무시

        currentHp -= damage;
        Debug.Log(gameObject.name + "남은 체력: " + currentHp);

        if (currentHp <= 0)
        {
            Die(); 
            
        }
    }

    // 스포너에서 호출해서 번호 설정
    public void SetSpawnNumber(int number)
    {
        SpawnNumber = number;
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

        // 스폰 매니저에 알리기
        SpawnManager spawnManager = FindAnyObjectByType<SpawnManager>();
        if (spawnManager != null)
        {
            spawnManager.OnMonsterDie();
        }
        

        Destroy(gameObject);
    }


 }