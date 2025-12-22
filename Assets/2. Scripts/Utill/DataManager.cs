using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DataManager : MonoBehaviour
{
    private static DataManager instance;

    public static DataManager Instance { get => instance; private set => instance = value; }

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

        DataParse();
    }

    #region Data Table

    public Dictionary<int, StageData>                   StageData               = new Dictionary<int, StageData>();
    public Dictionary<int, WaveData>                    WaveData                = new Dictionary<int, WaveData>();
    public Dictionary<int, MonsterSpawnGroupData>       MonsterSpawnGroupData   = new Dictionary<int, MonsterSpawnGroupData>();
    public Dictionary<int, TileData>                    TileData                = new Dictionary<int, TileData>();
    public Dictionary<int, MonsterData>                 MonsterData             = new Dictionary<int, MonsterData>();
    public Dictionary<int, RewardData>                  RewardData              = new Dictionary<int, RewardData>();
    public Dictionary<int, RewardGroupData>             RewardGroupData         = new Dictionary<int, RewardGroupData>();
    public Dictionary<int, TowerBaseData>               TowerBaseData           = new Dictionary<int, TowerBaseData>();
    public Dictionary<int, GachaData>                   GachaData               = new Dictionary<int, GachaData>();
    public Dictionary<int, DebuffData>                  DebuffData              = new Dictionary<int, DebuffData>();
    public Dictionary<int, AugmentData>                 AugmentData             = new Dictionary<int, AugmentData>();
    public Dictionary<string, LocalizationData>         LocalizationData        = new Dictionary<string, LocalizationData>();

    #endregion

    private void DataParse()
    {

        // 스테이지 및 웨이브
        AddToDictionary(StageData, DataParser.Parse<StageData>("stage"), d => d.Index);
        AddToDictionary(WaveData, DataParser.Parse<WaveData>("wave"), d => d.Index);

        // 몬스터 관련
        AddToDictionary(MonsterSpawnGroupData, DataParser.Parse<MonsterSpawnGroupData>("mon_spawn_groups"), d => d.Index);
        AddToDictionary(MonsterData, DataParser.Parse<MonsterData>("monsters"), d => d.MonsterId);

        // 타일 및 환경
        AddToDictionary(TileData, DataParser.Parse<TileData>("tiles"), d => d.Index);

        // 보상 관련
        AddToDictionary(RewardData, DataParser.Parse<RewardData>("rewards"), d => d.RewardId);
        AddToDictionary(RewardGroupData, DataParser.Parse<RewardGroupData>("reward_groups"), d => d.Index);

        // 타워 및 전투
        AddToDictionary(TowerBaseData, DataParser.Parse<TowerBaseData>("tower_base"), d => d.TowerID);
        AddToDictionary(GachaData, DataParser.Parse<GachaData>("gacha"), d => d.Index);
        AddToDictionary(DebuffData, DataParser.Parse<DebuffData>("debuff"), d => d.DebuffId);

        // 증강
        AddToDictionary(AugmentData, DataParser.Parse<AugmentData>("augment"), d=> d.Index);

        AddToDictionary(LocalizationData, DataParser.Parse<LocalizationData>("localization"), d=> d.Index);

        Debug.Log("모든 데이터 파싱 완료");
    }

    private void AddToDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, List<TValue> list, System.Func<TValue, TKey> keySelector)
    {
        if (list == null) return;

        foreach (var item in list)
        {
            TKey key = keySelector(item);
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, item);
            }
            else
            {
                Debug.LogWarning($"중복된 키 발견: {key} in {typeof(TValue).Name}");
            }
        }
    }

}
