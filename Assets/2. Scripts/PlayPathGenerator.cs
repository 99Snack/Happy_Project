using UnityEngine;

public class PlayPathGenerator : MonoBehaviour
{
    private static PlayPathGenerator instance;
    public static PlayPathGenerator Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }



}
