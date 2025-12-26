using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// 스테이지/웨이브 UI 담당 
/// </summary>
public class StageUICrtl : MonoBehaviour
{
    [Header("웨이브 준비 UI")]
    public GameObject ReadyPanel;
    public GameObject StartButton;

    [Header("결과 메시지 UI")]
    public GameObject ResultPanel;
    public Text ResultText;

    [Header("스테이지 결과 팝업")]
    public GameObject Background;
    public GameObject ClearPopup;
    public GameObject FailPopup;

    [Header("씬 설정")]
    public string LobbySceneName = "Lobby";

    private float messageTime = 3f; // 결과 메시지 노출 시간 

    private void Start()
    {
        HideAllPopups();
        ShowReadyUI();
    }

    /// <summary>
    ///  준비 ui 표시 (SpawnManager.OnNextWaveReady에 연결)
    /// </summary>
    public void ShowReadyUI()
    {
        if (ReadyPanel != null) ReadyPanel.SetActive(true);
        if (StartButton != null) StartButton.SetActive(true);
        if (ResultPanel != null) ResultPanel.SetActive(false);
    }

    /// <summary>
    /// 전투 UI 표시 (SpawnManager.OnWaveStart에 연결)
    public void ShowBattleUI()
    {
        if (ReadyPanel != null) ReadyPanel.SetActive(false);
        if (StartButton != null) StartButton.SetActive(false);
    }

    // 결과 메시지
    
    // 승리 메시지 
    public void ShowWinMessage()
    {
        StartCoroutine(ShowResultMessageCoroutine("Wave Clear"));
    }

    // 패배 메시지
    public void ShowLoseMessage()
    {
        StartCoroutine(ShowResultMessageCoroutine("Wave Lose"));
    }

    private IEnumerator ShowResultMessageCoroutine(string message)
    {
        // 메시지 표시
        if (ResultPanel != null) ResultPanel.SetActive(true);
        if (ResultText != null) ResultText.text = message;

        // 일정 시간 대기
        yield return new WaitForSeconds(messageTime);
        
        // 메시지 숨김 
        if (ResultPanel != null) ResultPanel.SetActive(false);
        
        // 스폰 매니저에 알림 
        if (SpawnManager.Instance != null)
        {
            SpawnManager.Instance.OnResultMessageEnd(); 
        }
    }

    /// <summary>
    /// 결과 팝업 
    /// </summary>
    
    // 클리어 팝업  
    public void ShowClearPopup()
    {
        StartCoroutine(ShowPopupAfterDelay(ClearPopup));
    }

    // 실패 팝업 
    public void ShowFailPopup()
    {
        StartCoroutine(ShowPopupAfterDelay(FailPopup));
    }
    private IEnumerator ShowPopupAfterDelay(GameObject popup)
    {
        yield return new WaitForSeconds(messageTime); // 3초 대기. 메시지 표시 시간

        // 팝업 표시
        if (Background != null) Background.SetActive(true);
        if (popup != null) popup.SetActive(true);

        Time.timeScale = 0f; // 게임 일시정지 
    }
    private void HideAllPopups()
    {
        if (ResultPanel != null) ResultPanel.SetActive(false);
        if (ClearPopup != null) ClearPopup.SetActive(false);
        if (FailPopup != null) FailPopup.SetActive(false);
        if (Background != null) Background.SetActive(false);
    }

    // 나가기 버튼 
    public void OnClickExit()
    {
        Time.timeScale = 1f; // 시간 정상화
        SceneManager.LoadScene(LobbySceneName);
    }

    // 재시작 버튼 
    public void OnClickRestart()
    {
        Time.timeScale = 1f; // 시간 정상화
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
