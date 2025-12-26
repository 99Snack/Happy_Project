using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events; 

public class SpawnManager : MonoBehaviour
{
    // Preparation : 웨이브 준비 상태
    // InProgress : 웨이브 진행 상태 
    public enum STATE { Preparation, InProgress }

    private static SpawnManager instance;
    public static SpawnManager Instance => instance;

    // 타일 건설 가능 여부
    public bool CanBuild => currentState == STATE.Preparation;

    [Header("몬스터 프리팹")]
    public GameObject[] MonsterPrefabs; // 생성할 몬스터 프리팹

    //[Header("스폰 위치")]
    //public Transform SpawnPoint; // 빈 오브젝트로 몬스터 스폰 위치 지정
    //[Header("타겟 위치")]
    //public Transform TargetPoint; // 베이스캠프 오브젝트 연결 

    [Header("UI")]
    public Text StageText;

    [Header("스폰 설정")]
    public float StartDelay = 2f; // 웨이브 시작 대기 시간, 스폰 시작 지연 
    public float SpawnInterval = 1f; // 몬스터 스폰 간격

    [Header("스테이지 데이터")]
    //public StageData[] Stages;
    public StageFakeData[] Stages;

    [Header("웨이브 설정")]
    public int TotalWaves = 15;
    private int waveIndex = 0; // 현재 웨이브 인덱스

    // 외부에서 읽기용
    public int CurrentWaveDisplay => waveIndex + 1;
    public int CurrentStageDisplay => currentStage + 1;

    [Header("웨이브 이벤트 (인스펙터에서 연결)")]
    public UnityEvent OnWaveStart;      // 웨이브 시작 시
    public UnityEvent OnWaveWin;        // 웨이브 승리 시
    public UnityEvent OnWaveLose;       // 웨이브 패배 시
    public UnityEvent OnStageClear;     // 스테이지 클리어 시
    public UnityEvent OnStageFail;      // 스테이지 실패 시
    public UnityEvent OnNextWaveReady;  // 다음 웨이브 준비 시

    // 현재 상태
    private STATE currentState = STATE.Preparation;
    private int currentStage = 0; // 현재 스테이지 번호 (0부터 시작, 인덱스 0 = 스테이지 1) 
    // 깃이슈에 스테이지당 웨이브의 스폰순서 등이 없어서 나중에 db작업할때 리팩토링 

    // 스폰 관련 
    private int spawnCount = 0; // 현재 스폰된 몬스터 수
    private int orderIndex = 0; // 스폰 순서 인덱스 
    private int[] currentSpawnOrder; // 현재 웨이브의 스폰 순서 배열
    private int aliveMonsterCount = 0; // 현재 살아있는 몬스터 수

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    private void Start()
    {
        currentState = STATE.Preparation; // 대기 상태 
        waveIndex = 0; 
        UpdateStageUI();
    }

    // UI 버튼에서 호출할 메서드 
    public void OnStartButtonClick()
    {
        //if (currentState == STATE.Preparation)
        //{
        //    StartWave();  // Idle 또는 Maintenance 상태에서만 시작 가능
        //}
        if (currentState != STATE.Preparation) return;
        OnWaveStart.Invoke(); // 웨이브 시작 이벤트 호출
        StartWave();

    }

    private void StartWave()
    {
        if (waveIndex >= Stages.Length)
        {
            Debug.Log("모든 스테이지를 클리어하여 더 이상 진행할 수 없습니다.");
            return; 
        }

        currentState = STATE.InProgress; // 웨이브 단계로 전환   

        // 스폰 초기화
        spawnCount = 0;
        orderIndex = 0;


        // 현재 스테이지 데이터로 스폰 순서 가져와서 설정
        StageFakeData currentData = Stages[waveIndex]; // 현재 스테이지 데이터 가져오기
        currentSpawnOrder = currentData.GetSpawnOrder(); // 스폰 순서 배열 생성
        aliveMonsterCount = currentSpawnOrder.Length; // 살아있는 몬스터 수 설정

        UpdateStageUI();
        Debug.Log("스테이지 " + (currentStage + 1) + " 시작, 총 몬스터 수: " + aliveMonsterCount);

        // Start Delay 후에 첫 스폰 시작 
        InvokeRepeating("SpawnOne", StartDelay, SpawnInterval); // 웨이브 시작 지연 후 스폰 반복 시작
    }

    private void SpawnOne()
    {
        if (orderIndex >= currentSpawnOrder.Length)
        {
            CancelInvoke("SpawnOne"); // 스폰 반복 취소
            Debug.Log("모든 몬스터 스폰 완료, 몬스터 처치 대기중");
            return; // 스폰 순서 끝났으면 종료
        }

        spawnCount++; // 스폰 된 몬스터 번호 증가 


        int spawnValue = currentSpawnOrder[orderIndex];
        GameObject prefab = null;
        MonsterData data = null;

        // DB에서 몬스터 데이터 찾기 (DB 우선, 없으면 기존 인스펙터 프리팹 사용)
        if (DataManager.Instance != null &&
            DataManager.Instance.MonsterData.TryGetValue(spawnValue, out data))
        {
            // DB에서 찾음 -> MonsterResource로 프리팹 로드 
            prefab = Resources.Load<GameObject>(data.MonsterResource);
            
            if (prefab == null)
            {
                Debug.LogError("프리팹 못찾음:" + data.MonsterResource);
           
            }
            else
            {
                Debug.Log("DB에서 프리팹 로드 성공: " + data.MonsterResource);
            }
        }
        // DB에서 못 찾았거나 프리팹 로드 실패 시 -> 기존 프리팹 배열 사용 
        if (prefab == null)
        {
            if (spawnValue >= 0 && spawnValue < MonsterPrefabs.Length)
            {  

                prefab = MonsterPrefabs[spawnValue];
                Debug.Log("기존 인스펙터 프리팹 사용: " + prefab.name);
            }
            else
            {
                Debug.LogError("프리팹 못찾음: " + spawnValue);
                orderIndex++;
                return; // 잘못된 인덱스면 종료
            }
            
        }

        //int monsterType = currentSpawnOrder[orderIndex]; // 현재 스폰할 몬스터 타입 가져오기
        //GameObject prefab = MonsterPrefabs[monsterType]; // 몬스터 프리팹 선택

        // 타일매니저의 적 베이스 위치에서 월드 좌표 가져와서 스폰 위치로 사용
        Vector3 spawnPos = TileManager.Instance.GetWorldPosition(TileManager.Instance.enemyBasePosition);

        GameObject monster = Instantiate(prefab, spawnPos, Quaternion.identity); // 몬스터 생성

        Monster monsterScript = monster.GetComponent<Monster>();
        monsterScript.SetSpawnNumber(spawnCount); // 스폰 번호 설정


        // DB 에서 몬스터 데이터 설정, DB 데이터가 있어야 할당 
        if (data != null)
        {
            monsterScript.Data =data; // DB에서 가져온 몬스터 데이터 설정
        }

        Debug.Log("스폰 : " + prefab.name + "(번호 : " + spawnCount + ")");

        orderIndex++;
    }

    // 몬스터 사망 시 Monster.Die()에서 호출
    public void OnMonsterDie()
    {
        aliveMonsterCount--; // 살아있는 몬스터 수 감소
        Debug.Log("남은 몬스터 수: " + aliveMonsterCount);

        // 몬스터 전멸하면 정비 단계로 전환
        if (aliveMonsterCount <= 0 && currentState == STATE.InProgress)
        {
            ProcessWaveWin();
        }
    }

    // 웨이브 승리 처리 
    private void ProcessWaveWin()
    {
        Debug.Log("웨이브 승리" + (waveIndex + 1));
        OnWaveWin.Invoke();
        bool isLastWave = (waveIndex >= TotalWaves - 1);
        if (isLastWave)
        {
            Debug.Log("모든 스테이지 클리어!");
            OnStageClear.Invoke();
        }
        else
        {
            waveIndex++;
            currentState = STATE.Preparation;
            Debug.Log("스테이지 클리어! 정비 단계로 전환");
            UpdateStageUI();
        }
    }

    // 메시지 끝난 후 호출 (StageUICrtl에서 호출)
    public void OnResultMessageEnd()
    {
       if(waveIndex < TotalWaves)
        {
            OnNextWaveReady.Invoke();
        }
    }

    // 패배 처리 (BaseCamp에서 호출)
    public void OnWaveDefeat()
    {
        if (currentState != STATE.InProgress) return;
        currentState = STATE.Preparation;
        CancelInvoke("SpawnOne"); // 스폰 중지
        Debug.Log("스테이지 실패!"); 
        OnWaveLose.Invoke();
        OnStageFail.Invoke();
    }

    private void UpdateStageUI()
    {
        if (StageText != null)
        {
            //int displayStage = currentStage + 1; // 사용자에게는 1부터 표시
            //StageText.text = "Stage: " + displayStage;
            StageText.text = "Stage" + CurrentStageDisplay + " - Wave" + CurrentWaveDisplay +"/" + TotalWaves; 
        }
    }

}
