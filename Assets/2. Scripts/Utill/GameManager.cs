using System;
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

    //초기 재화 300골드 지급
    public static readonly int START_GOLD = 300;

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

    private void Start()
    {
        gold = START_GOLD;
    }


}
