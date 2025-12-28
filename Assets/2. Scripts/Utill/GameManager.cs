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

    public int MeleeBonusGold{ get; set; }
    public int RangeBonusGold{ get; set; }
    #endregion

    private StageData stageInfo;
    public StageData StageInfo { get => stageInfo; set => stageInfo = value; }

    private WaveData waveInfo;
    public WaveData WaveInfo
    {
        get => waveInfo; set
        {
            waveInfo = value;
            BaseCamp.Instance.SetUp(WaveInfo.Index);
        }
    }



    private void Start()
    {
        Gold = START_GOLD;
    }


}
