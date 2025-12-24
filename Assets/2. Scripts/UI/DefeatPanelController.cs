using UnityEngine;

public class DefeatPanelController : MonoBehaviour
{
    public GameObject Pnl_defeat_Screen;
    public GameObject popupLayer;

    public void ShowDefeatPanel()
    {
        Pnl_defeat_Screen.SetActive(true);
        popupLayer.SetActive(false);
    }
}
