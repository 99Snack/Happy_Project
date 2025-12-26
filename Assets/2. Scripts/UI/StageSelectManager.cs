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
        }
    }

    StageState GetStageState(int index, bool hasStageData)
    {
#if UNITY_EDITOR
        // CSV 없을 때만 디버그 상태 허용
        if (!hasStageData && debugMixedState)
        {
            if (index <= debugCompletedUntil)
                return StageState.Completed;

            if (index <= debugAvailableUntil)
                return StageState.Available;

            return StageState.Locked;
        }
#endif
        // 실제 로직
        bool isCleared = PlayerPrefs.GetInt($"StageClear_{index}", 0) == 1;
        bool isPrevCleared = PlayerPrefs.GetInt($"StageClear_{index - 1}", 0) == 1;

        if (isCleared)
            return StageState.Completed;

        if (index == 1 || isPrevCleared)
            return StageState.Available;

        return StageState.Locked;
    }
}
