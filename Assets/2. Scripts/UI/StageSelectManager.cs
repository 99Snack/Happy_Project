using UnityEngine;
using System.Linq;

public class StageSelectManager : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] bool debugMixedState = true;
    [SerializeField] int debugCompletedUntil = 2;  // 1~2번 Completed
    [SerializeField] int debugAvailableUntil = 4;  // 3~4번 Available

    [Header("Stage")]
    public Stage stagePrefab;       // Stage.cs 붙은 프리팹
    public Transform stageParent;   // Grp_Stage_Butten

    void Start()
    {
        if (UIManager.Instance.stageTrans != null)
        {
            Initialize();
        }
        else
        {
            UIManager.Instance.OnUIInitialized += Initialize;
        }
    }

    void Initialize()
    {
        UIManager.Instance.OnUIInitialized -= Initialize;
        stageParent = UIManager.Instance.stageTrans;
        CreateStages();

    }

    void CreateStages()
    {
        var stageTable = DataManager.Instance != null
            ? DataManager.Instance.StageData
            : null;

        // CSV 없을 때  테스트용 버튼 생성
        if (stageTable == null || stageTable.Count == 0)
        {
            Debug.Log("StageData 없음 → 테스트용 버튼 생성");

            for (int i = 1; i <= 5; i++)
            {
                Stage stage = Instantiate(stagePrefab, stageParent);
                stage.stageIndex = i;
                stage.stageState = GetStageState(i, false); // CSV 없음
            }
            return;
        }

        // CSV 있을 때  실제 데이터 기반 생성
        var orderedStages = stageTable.Values.OrderBy(s => s.Index);
        foreach (var stageData in orderedStages)
        {
            Stage stage = Instantiate(stagePrefab, stageParent);
            stage.stageIndex = stageData.Index;
            stage.stageState = GetStageState(stageData.Index, true); // CSV 있음
            stage.RefreshUI();

            if (stage.stageIndex == 1)
            {
                Debug.Log("[StageSelectManager] 첫 스테이지 확인: StageIndex=1, State=" + stage.stageState);
            }
            else
            {
                Debug.Log("[StageSelectManager] Stage 생성: StageIndex=" + stage.stageIndex + ", State=" + stage.stageState);
            }

        }
    }

    StageState GetStageState(int index, bool hasStageData)
    {
#if UNITY_EDITOR
        if (!hasStageData && debugMixedState)
        {
            if (index <= debugCompletedUntil)
                return StageState.Completed;

            if (index <= debugAvailableUntil)
                return StageState.Available;

            return StageState.Locked;
        }
#endif

        bool isCleared = PlayerPrefs.GetInt($"StageClear_{index}", 0) == 1;

        // CSV 있을 때 첫 Stage 판정
        bool isFirstStage = false;
        if (DataManager.Instance != null && DataManager.Instance.StageData != null)
        {
            int minIndex = DataManager.Instance.StageData.Values.Min(s => s.Index);
            if (index == minIndex)
                isFirstStage = true;
        }
        else
        {
            if (index == 1) // CSV 없을 때 기존 로직 유지
                isFirstStage = true;
        }

        bool isPrevCleared = PlayerPrefs.GetInt($"StageClear_{index - 1}", 0) == 1;

        if (isCleared)
            return StageState.Completed;

        if (isFirstStage || isPrevCleared)
            return StageState.Available;

        return StageState.Locked;
    }

}
