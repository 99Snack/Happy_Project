using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    // Preparation : 웨이브 준비 상태
    // InProgress : 웨이브 진행 상태 
    public enum STATE { Preparation, InProgress }

    private static SpawnManager instance;
    public static SpawnManager Instance => instance;

    //정비 가능 여부
    /// <summary>
    /// true : 준비시간, false : 게임 시작함
    /// </summary>
    public bool CanMaintain => currentState == STATE.Preparation;

    [Header("몬스터 프리팹")]
    public GameObject[] MonsterPrefabs; // 생성할 몬스터 프리팹

    [Header("스폰 설정")]
    public float StartDelay = 2f; // 웨이브 시작 대기 시간, 스폰 시작 지연 
    public float SpawnInterval = 1f; // 몬스터 스폰 간격
    public bool isAuto = false;
    private Coroutine spawnRoutine = null;

    [Header("웨이브 설정")]
    public int TotalWaves = 15;
    private int waveIndex = 0; // 현재 웨이브 인덱스
    private WaveData currentWaveInfo = new WaveData();
    private List<MonsterSpawnGroupData> currentMonsterGropInfo = new List<MonsterSpawnGroupData>();
    private List<WaveData> waves = new List<WaveData>();

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

    // 스폰 관련 
    private int spawnCount = 0; // 현재 스폰된 몬스터 수
    private int orderIndex = 0; // 스폰 순서 인덱스 
    private int currentSpawnCount; //스폰 해야할 몬스터 수
    private int aliveMonsterCount = 0; // 현재 살아있는 몬스터 수
    private List<Monster> activeMonsterList = new List<Monster>();

    private Coroutine WaveResultRoutine = null;

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
        ResetStage();
    }

    // UI 버튼에서 호출할 메서드 
    public void OnStartButtonClick()
    {
        if (currentState != STATE.Preparation) return;

        //if (currentState == STATE.Preparation)
        //{
        //    StartWave();  // Idle 또는 Maintenance 상태에서만 시작 가능
        //}

        //정비 패널 닫기
        UIManager.Instance.CloseWavePreparationPanel();
        //웨이브 진행 중일때의 패널 열기
        UIManager.Instance.OpenAllyBaseCampPanel();

        OnWaveStart.Invoke(); // 웨이브 시작 이벤트 호출
        StartWave();
    }

    private void StartWave()
    {
        if (waveIndex >= DataManager.Instance.WaveData.Count)
        {
            Debug.Log("모든 스테이지를 클리어하여 더 이상 진행할 수 없습니다.");
            return;
        }

        currentState = STATE.InProgress; // 웨이브 단계로 전환   

        // 스폰 초기화
        spawnCount = 0;

        UIManager.Instance.UpdateWaveSlider(spawnCount, waves.Count);

        // 현재 스테이지 데이터로 스폰 순서 가져와서 설정
        currentWaveInfo = waves[waveIndex];
        StartDelay = currentWaveInfo.SpawnStartDelay_ms;
        SpawnInterval = currentWaveInfo.SpawnInterval_ms;

        //todo : 임의 스폰데이터 가져오기
        currentMonsterGropInfo = DataManager.Instance.MonsterSpawnGroupData[currentWaveInfo.MonsterSpawnGroup];

        //StageFakeData currentData = Stages[waveIndex]; // 현재 스테이지 데이터 가져오기
        //currentSpawnOrder = currentData.GetSpawnOrder(); // 스폰 순서 배열 생성
        currentSpawnCount = currentMonsterGropInfo.Sum(x => x.MonsterCount);
        aliveMonsterCount = currentSpawnCount; // 살아있는 몬스터 수 설정

        UpdateStageUI();
        //Debug.Log("스테이지 " + (currentStage + 1) + " 시작, 총 몬스터 수: " + aliveMonsterCount);

        // Start Delay 후에 첫 스폰 시작 
        //for (int i = 0; i < currentMonsterGropInfo.Count; i++)
        //{
        //    InvokeRepeating("SpawnOne", StartDelay, SpawnInterval); // 웨이브 시작 지연 후 스폰 반복 시작
        //}
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
        spawnRoutine = StartCoroutine(SpawnWaveRoutine(currentMonsterGropInfo, StartDelay, SpawnInterval));
    }

    IEnumerator SpawnWaveRoutine(List<MonsterSpawnGroupData> monsterGroups, float startDelay, float interval)
    {
        yield return new WaitForSeconds(startDelay);

        foreach (var group in monsterGroups)
        {
            for (int i = 0; i < group.MonsterCount; i++)
            {
                SpawnOne(group.MonsterId);
                yield return new WaitForSeconds(interval);
            }
            orderIndex++;
        }
    }

    private void SpawnOne(int monsterID)
    {
        //if (spawnCount >= currentSpawnCount)
        //{
        //    CancelInvoke("SpawnOne"); // 스폰 반복 취소
        //    Debug.Log("모든 몬스터 스폰 완료, 몬스터 처치 대기중");
        //    return; // 스폰 순서 끝났으면 종료
        //}

        ////int spawnValue = currentSpawnOrder[orderIndex];
        //GameObject prefab = null;
        //MonsterData monsterData = DataManager.Instance.MonsterData[monsterID];

        //// DB에서 몬스터 데이터 찾기 (DB 우선, 없으면 기존 인스펙터 프리팹 사용)
        //if (monsterData != null)
        //{
        //    // DB에서 찾음 -> MonsterResource로 프리팹 로드 
        //    prefab = Resources.Load<GameObject>($"Prefab/Enemy/{monsterID}");

        //    if (prefab == null)
        //    {
        //        Debug.LogError("프리팹 못찾음:" + monsterData.MonsterResource);
        //    }
        //    else
        //    {
        //        LocalizationData local = DataManager.Instance.LocalizationData[monsterData.MonsterName];
        //        //Debug.Log("DB에서 프리팹 로드 성공: " + local.Ko);
        //    }
        //}
        //// DB에서 못 찾았거나 프리팹 로드 실패 시 -> 기존 프리팹 배열 사용 
        ////if (prefab == null)
        ////{
        ////    if (spawnValue >= 0 && spawnValue < MonsterPrefabs.Length)
        ////    {

        ////        prefab = MonsterPrefabs[spawnValue];
        ////        //Debug.Log("기존 인스펙터 프리팹 사용: " + prefab.name);
        ////    }
        ////    else
        ////    {
        ////        Debug.LogError("프리팹 못찾음: " + spawnValue);
        ////        orderIndex++;
        ////        return; // 잘못된 인덱스면 종료
        ////    }

        ////}

        ////int monsterType = currentSpawnOrder[orderIndex]; // 현재 스폰할 몬스터 타입 가져오기
        ////GameObject prefab = MonsterPrefabs[monsterType]; // 몬스터 프리팹 선택

        //// 타일매니저의 적 베이스 위치에서 월드 좌표 가져와서 스폰 위치로 사용
        //Vector3 spawnPos = TileManager.Instance.GetWorldPosition(TileManager.Instance.enemyBasePosition);

        //GameObject monster = Instantiate(prefab, spawnPos, Quaternion.identity); // 몬스터 생성

        //Monster monsterScript = monster.GetComponent<Monster>();
        ////Debug.Log($"test : {currentMonsterGropInfo.Count} : {orderIndex}");
        //monsterScript.SetSpawnNumber(monsterData, currentMonsterGropInfo[orderIndex].SpawnOrder); // 스폰 번호 설정

        ////Debug.Log("스폰 : " + prefab.name + "(번호 : " + _spawnCount + ")");

        //spawnCount++;

        //UIManager.Instance.UpdateWaveSlider(spawnCount, waves.Count);


        //===================================
        //if (spawnCount >= currentSpawnCount)
        //{
        //    CancelInvoke("SpawnOne");
        //    Debug.Log("모든 몬스터 스폰 완료");
        //    return;
        //}

        MonsterData monsterData = DataManager.Instance.MonsterData[monsterID];
        if (monsterData == null) return;

        string monsterTag = monsterID.ToString();

        Vector3 spawnPos = TileManager.Instance.GetWorldPosition(TileManager.Instance.enemyBasePosition);

        GameObject monster = ObjectPoolManager.Instance.SpawnFromPool(monsterTag, spawnPos, Quaternion.identity);

        if (monster != null)
        {
            Monster monsterScript = monster.GetComponent<Monster>();

            monsterScript.SetSpawnNumber(monsterData, currentMonsterGropInfo[orderIndex].SpawnOrder);
            activeMonsterList.Add(monsterScript);

            spawnCount++;

            UIManager.Instance.UpdateWaveSlider(spawnCount, currentSpawnCount);
        }
    }

    // 몬스터 사망 시 Monster.Die()에서 호출
    public void OnMonsterDie(Monster monster)
    {
        activeMonsterList.Remove(monster); // 리스트에서 제거

        aliveMonsterCount--;

        if (aliveMonsterCount <= 0 && currentState == STATE.InProgress)
        {
            if (WaveResultRoutine != null) WaveResultRoutine = null;
            WaveResultRoutine = StartCoroutine(ProcessWaveWin());
        }
    }

    // 웨이브 승리 처리 
    IEnumerator ProcessWaveWin()
    {
        UIManager.Instance.OpenWaveResultPanel(1);
        yield return new WaitWhile(() => UIManager.Instance.IsActiveWaveResultPanel());

        //Debug.Log("웨이브 승리" + (waveIndex + 1));
        OnWaveWin.Invoke();
        //bool isLastWave = (waveIndex >= TotalWaves - 1);
        bool isLastWave = (waveIndex >= 0);
        if (isLastWave)
        {
            //Debug.Log("모든 스테이지 클리어!");
            OnStageClear.Invoke();

            UIManager.Instance.OpenStageResultPanel(1);
        }
        else
        {
            waveIndex++;
            currentState = STATE.Preparation;

            int nextStageNum = waveIndex + 1;
            if (nextStageNum == 4 || nextStageNum == 9)
            {
                UIManager.Instance.OpenAugmentPanel(nextStageNum);
            }

            //Debug.Log("스테이지 클리어! 정비 단계로 전환");
            UIManager.Instance.OpenWavePreparationPanel();
            UIManager.Instance.CloseAllyBaseCampPanel();
            UpdateStageUI();
        }
    }

    // 메시지 끝난 후 호출 (StageUICrtl에서 호출)
    public void OnResultMessageEnd()
    {
        if (waveIndex < waves.Count)
        {
            OnNextWaveReady.Invoke();
        }
    }

    public void OnWaveDefeat()
    {
        if (currentState != STATE.InProgress) return;

        currentState = STATE.Preparation;
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        Debug.Log("스테이지 패배!");

        RecallAllMonsters();

        if (WaveResultRoutine != null)
        {
            StopCoroutine(WaveResultRoutine);
            WaveResultRoutine = null;
        }
        WaveResultRoutine = StartCoroutine(ProcessWaveLoss());
    }

    IEnumerator ProcessWaveLoss()
    {
        UIManager.Instance.OpenWaveResultPanel(0);

        yield return new WaitWhile(() => UIManager.Instance.IsActiveWaveResultPanel());


        UIManager.Instance.OpenStageResultPanel(0);
        yield return new WaitWhile(() => UIManager.Instance.IsActiveStageResultPanel());
        ResetStage();
    }

    public void RecallAllMonsters()
    {
        for (int i = activeMonsterList.Count - 1; i >= 0; i--)
        {
            Monster m = activeMonsterList[i];
            string tag = m.Data.MonsterId.ToString();

            ObjectPoolManager.Instance.ReturnToPool(tag, m.gameObject);
        }

        activeMonsterList.Clear();
        aliveMonsterCount = 0;
    }

    private void UpdateStageUI()
    {
        UIManager.Instance.UpdateStageInfo(waves[waveIndex]);
    }

    public void ResetStage()
    {
        currentState = STATE.Preparation; // 대기 상태 

        //Debug.Log("스테이지 클리어! 정비 단계로 전환");
        UIManager.Instance.OpenWavePreparationPanel();
        UIManager.Instance.CloseAllyBaseCampPanel();
        
        waveIndex = 0;
        orderIndex = 0;
        spawnCount = 0;
        aliveMonsterCount = 0;

        //todo : 리셋시 제거해야할 목록
        //증강 제거
        //증강 액티베이트 슬롯 제거
        //타워 제거
        //타워 매니저에서 제거
        //타일도 초기화해야 되고(isAlreadyTower=true)false로
        //골드 초기화

        //StageData stageInfo = GameManager.Instance.StageInfo;
        //todo : 임의스테이지 삽입 로비씬에서의 게임매니저와 인게임씬 안의 게임매니저가 달라서 씬정보가 넘어오지 않음
        StageData stageInfo = DataManager.Instance.StageData[10001];

        //웨이브 정보가 없을 때
        if (waves.Count == 0)
        {
            waves = DataManager.Instance.WaveData.Values
            .Where(x => x.WaveGroup.Equals(stageInfo.WaveGroup))
            .OrderBy(x => x.WaveOrder).ToList();
        }

        BaseCamp.Instance.SetUp(waves[waveIndex].Index);

        TotalWaves = waves.Count;

        GameManager.Instance.WaveInfo = waves[waveIndex];

        UpdateStageUI();
        UIManager.Instance.OpenAugmentPanel(1);
    }

}
