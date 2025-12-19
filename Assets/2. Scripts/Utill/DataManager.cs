using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DataManager : MonoBehaviour
{
    private static DataManager instance;

    public static DataManager Instance { get => instance; set => instance = value; }

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

    #region Tower Table

    public Dictionary<int, Gacha> GachaData = new Dictionary<int, Gacha>();
    public Dictionary<int, Tower_Base> TowerBaseData = new Dictionary<int, Tower_Base>();

    #endregion

    private void Start()
    {
        //1. 모든파일들 파서 진행
        List<Gacha> gaches = DataParser.Parse<Gacha>("Gacha");
        List<Tower_Base> towerBases = DataParser.Parse<Tower_Base>("Tower_Base");

        //Gacha 파서
        foreach (var gacha in gaches)
        {
            int primaryKey = gacha.Index;
            if (!GachaData.ContainsKey(primaryKey))
            {
                GachaData.Add(primaryKey, gacha);
            }
        }

        //Tower_Base 파서
        foreach (var tower in towerBases)
        {
            int primaryKey = tower.TowerID;
            if (!TowerBaseData.ContainsKey(primaryKey))
            {
                TowerBaseData.Add(primaryKey, tower);
            }
        }
    }

}
