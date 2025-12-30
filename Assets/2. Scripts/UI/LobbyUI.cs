using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private LobbyPanel lobbyPanel;
    [SerializeField] private StagePanel stagePanel;

    public void OpenStagePanel()
    {
        lobbyPanel.gameObject.SetActive(false);
        stagePanel.gameObject.SetActive(true);
    }

    public void CloseStagePanel()
    {
        lobbyPanel.gameObject.SetActive(true);
        stagePanel.gameObject.SetActive(false);
    }
}
