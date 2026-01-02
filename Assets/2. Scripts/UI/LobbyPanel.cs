using UnityEngine;

public class LobbyPanel : MonoBehaviour
{
    public GameObject gameExitPopup;

    public void OpenGameExitPopup()
    {
        SoundManager.Instance.PlaySFX(ClipName.Btn_sound);
        gameExitPopup.SetActive(true);
    }

    public void CloseGameExitPopup()
    {
        gameExitPopup.SetActive(false);
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
