using System;
using UnityEditor.Search;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance { get => instance; private set => instance = value; }

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

    #region 재화
    public event Action<int> OnChangedGold;

    private int gold;
    public int Gold
    {
        get => gold;
        set
        {
            gold = value;
            OnChangedGold?.Invoke(gold);
        }
    }
    #endregion


}
