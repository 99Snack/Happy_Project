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

    void Start()
    {
        currentHp = Data.Hp; // 시작 시 현재 체력 = 최대 체력 
    }

    // 스포너에서 호출해서 번호 설정
    public void SetSpawnNumber(int number)
    {
        SpawnNumber = number;
    }

    // 몬스터 죽을 때 호출 
    public void Die()
    {
        // 스폰 매니저에 알리기
        SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
        spawnManager.OnMonsterDie();

        Destroy(gameObject);
    }

 }