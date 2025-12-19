using System;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public static UIManager Instance { get => instance; set => instance = value; }


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

    public void Gacha(){
        Debug.Log(TowerManager.Instance.Gacha());
    }
}
