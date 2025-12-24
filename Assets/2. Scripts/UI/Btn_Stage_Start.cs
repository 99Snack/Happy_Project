using UnityEngine;
using UnityEngine.SceneManagement;

public class Btn_Stage_Start : MonoBehaviour
{
    public void StageStart()
    {
        SceneManager.LoadScene("Stage");
    }
}
