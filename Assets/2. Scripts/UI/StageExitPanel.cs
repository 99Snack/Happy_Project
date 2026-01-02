using UnityEngine;

public class StageExitPanel : MonoBehaviour
{
    public void GoToLobby()
    {
        UIManager.Instance.GoToLobby();
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
