using UnityEngine;

public class Btn_Exit : MonoBehaviour
{
    public GameObject popupLayer;
    public GameObject Pnl_defeat_Screen;

    public void ShowPopup()
    {
        popupLayer.SetActive(true);
        Pnl_defeat_Screen.SetActive(false);
    }

    public void ClosePopup()
    {
        popupLayer.SetActive(false);
    }
}
