using UnityEngine;
using UnityEngine.UI; 

public class SpawnManager : MonoBehaviour
{
   public enum STATE { Idle, Wave, Maintenance }

    [Header("몬스터 프리팹")]
    public GameObject[] MonsterPrefabs; // 생성할 몬스터 프리팹

    [Header("스폰 위치")]
    public Transform SpawnPoint; // 빈 오브젝트로 몬스터 스폰 위치 지정
    [Header("타겟 위치")]
    public Transform TargetPoint; // 베이스캠프 오브젝트 연결 

    [Header("UI")]
    public Text StageText;

    [Header("스폰 설정")]
    public float StartDelay = 2f; // 웨이브 시작 대기 시간, 스폰 시작 지연 
    public float SpawnInterval = 1f; // 몬스터 스폰 간격

    [Header("스테이지 데이터")]
    public StageData[] Stages;


    // 현재 상태
    private STATE _currentState = STATE.Idle;
    private int _currentStage = 0; // 현재 스테이지 번호(0부터 시작, 스테이지 1 = 인덱스0) 
 
    // 스폰 관련 
    private int _spawnCount = 0; // 현재 스폰된 몬스터 수
    private int _orderIndex = 0; // 스폰 순서 인덱스 (예: 벌1, 꽃2, 쥐3, 유령4) 
    private int[] _currentSpawnOrder; // 현재 웨이브의 스폰 순서 배열
    private int _aliveMonsterCount = 0; // 현재 살아있는 몬스터 수

    private void Start()
    {
        _currentState = STATE.Idle;
        UpdateStageUI(); 
    }

    // UI 버튼에서 호출할 메서드 
    public void OnStartButtonClick()
    {  
        if (_currentState == STATE.Idle || _currentState == STATE.Maintenance)
        {
            StartWave();  // Idle 또는 Maintenance 상태에서만 시작 가능
        }
    }

    private void StartWave()
    {
        if(_currentStage >= Stages.Length)
        {
            Debug.Log("모든 스테이지 완료");
            return; 
        }

        _currentState = STATE.Wave; // 웨이브 단계로 전환   
        
        // 스폰 초기화
        _spawnCount = 0;
        _orderIndex = 0;


        // 현재 스테이지 데이터로 스폰 순서 가져와서 설정
        StageData currentData = Stages[_currentStage]; // 현재 스테이지 데이터 가져오기
        _currentSpawnOrder = currentData.GetSpawnOrder(); // 스폰 순서 배열 생성
        _aliveMonsterCount = _currentSpawnOrder.Length; // 살아있는 몬스터 수 설정

        UpdateStageUI();
        Debug.Log("스테이지 " + (_currentStage + 1) + " 시작, 총 몬스터 수: " + _aliveMonsterCount);

        // Start Delay 후에 첫 스폰 시작 
        InvokeRepeating("SpawnOne", StartDelay, SpawnInterval); // 웨이브 시작 지연 후 스폰 반복 시작
    }

    private void SpawnOne()
    {
        if (_orderIndex >= _currentSpawnOrder.Length)
        {
            CancelInvoke("SpawnOne"); // 스폰 반복 취소
            Debug.Log("모든 몬스터 스폰 완료, 몬스터 처치 대기중");
            return; // 스폰 순서 끝났으면 종료
        }

        _spawnCount++; // 스폰 된 몬스터 번호 증가 

        int monsterType = _currentSpawnOrder[_orderIndex]; // 현재 스폰할 몬스터 타입 가져오기
        GameObject prefab = MonsterPrefabs[monsterType]; // 몬스터 프리팹 선택

        // 스폰 위치 결정
        Vector3 spawnPos = (SpawnPoint != null) ? SpawnPoint.position : transform.position; // 스폰 포인트 위치 사용, 없으면 매니저 위치 사용
        GameObject monster = Instantiate(prefab, spawnPos, Quaternion.identity); // 몬스터 생성

        Monster monsterScript = monster.GetComponent<Monster>();
        monsterScript.SetSpawnNumber(_spawnCount); // 스폰 번호 설정

        // 타겟 위치 설정
        MonsterMove moveScript = monster.GetComponent<MonsterMove>();
        moveScript.SetTarget(TargetPoint); // 타겟 위치 설정

        Debug.Log("스폰 : " + prefab.name + "(번호 : " + _spawnCount + ")");

        _orderIndex++;
    }

    // 스테이지 UI 업데이트, Monster.Die()에서 호출
    public void OnMonsterDie()
    {
        _aliveMonsterCount--; // 살아있는 몬스터 수 감소
        Debug.Log("남은 몬스터 수: " + _aliveMonsterCount);

        // 몬스터 전멸하면 정비 단계로 전환
        if (_aliveMonsterCount <= 0 && _currentState == STATE.Wave)
        {
            _currentState = STATE.Maintenance; // 유지보수 단계로 전환
            _currentStage++; // 다음 스테이지 단계 증가 
            Debug.Log("스테이지 클리어! 정비 단계로 전환");
            UpdateStageUI();
        }
    }
    private void UpdateStageUI()
    {
        if (StageText != null) 
        {
            int displayStage = _currentStage + 1; // 사용자에게는 1부터 표시
            StageText.text = "Stage: " + displayStage; 
        }
    }

}
