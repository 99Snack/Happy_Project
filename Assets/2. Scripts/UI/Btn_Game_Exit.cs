using UnityEngine;

public class Btn_Game_Exit : MonoBehaviour
{
    
    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터 Play 종료
        #else
        Application.Quit(); // 빌드 게임 종료
        #endif
    }
}

