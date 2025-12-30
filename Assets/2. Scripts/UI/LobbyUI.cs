using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private LobbyPanel lobbyPanel;
    [SerializeField] private StagePanel stagePanel;

    public void OpenStagePanel()
    {
        SoundManager.Instance.PlaySFX(ClipName.Btn_sound);
        lobbyPanel.gameObject.SetActive(false);
        stagePanel.gameObject.SetActive(true);
    }

    public void CloseStagePanel()
    {
        SoundManager.Instance.PlaySFX(ClipName.Btn_sound);

        lobbyPanel.gameObject.SetActive(true);
        stagePanel.gameObject.SetActive(false);
    }
}
