using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageResultPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button ExitBtn;

    /// <summary>
    /// 1: 승리, 0 : 패배
    /// </summary>
    /// <param name="result"></param>
    public void Setup(int result = 0)
    {
        bool waveResult = result switch
        {
            1 => true,
            _ => false,
        };

        restartBtn.gameObject.SetActive(!waveResult);

        resultText.text = waveResult ? "클리어!" : "패배!";
    }

    public void RestartButton()
    {
        //클릭 시 현재 스테이지를 종료하고 스테이지의 첫 웨이브 준비 시간 화면으로 즉시 전환된다.
        //SpawnManager.Instance.ResetStage();
        gameObject.SetActive(false);
    }

    public void ExitButton()
    {
        //클릭 시 스테이지 씬을 종료하고 로비 화면으로 이동한다.
        SceneManager.LoadScene("Lobby");
        gameObject.SetActive(false);
    }
}
