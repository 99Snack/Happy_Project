using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtonsController : MonoBehaviour
{
    public GameObject popupLayer;

    public void GoToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void GoToStage()
    {
        SceneManager.LoadScene("Stage");
    }

    public void ShowPopup()
    {
        popupLayer.SetActive(true);
    }

    public void ClosePopup()
    {
        popupLayer.SetActive(false);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터 Play 종료
#else
        Application.Quit(); // 빌드 게임 종료
#endif
    }

}
