using UnityEngine;
using UnityEngine.SceneManagement;

public class Btn_Game_Start : MonoBehaviour
{
    public void LobbyStart()
    {
        SceneManager.LoadScene("Lobby");
    }
}
