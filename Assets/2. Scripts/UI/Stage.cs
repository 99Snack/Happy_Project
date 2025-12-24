using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public enum StageState
{
    Locked,     // 미해금
    Available,  // 진행 가능
    Completed   // 완료
}

public class Stage : MonoBehaviour
{
    public int stageIndex;          // 스테이지 번호
    public StageState stageState;   // 현재 스테이지 상태

    public Button button;
    public GameObject lockIcon;     // 잠금 아이콘
    public GameObject completeIcon; // 완료 아이콘

    private void Start()
    {
        if (button == null)
            button = GetComponent<Button>();

        RefreshUI();
        button.onClick.AddListener(OnStageButtonClicked);
    }

    void RefreshUI()
    {
        // 버튼 클릭 가능 여부
        button.interactable = (stageState != StageState.Locked);

        // 잠금 아이콘 (있을 때만)
        if (lockIcon != null)
            lockIcon.SetActive(stageState == StageState.Locked);

        // 완료 아이콘 (있을 때만)
        if (completeIcon != null)
            completeIcon.SetActive(stageState == StageState.Completed);
    }


    void OnStageButtonClicked()
    {
        if (stageState == StageState.Locked)
            return;

        EnterInGameUI();
    }

    void EnterInGameUI()
    {
        Debug.Log($"[StageButton] Stage {stageIndex} 진입");
        SceneManager.LoadScene("InGame");
    }
}
