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
    private readonly Dictionary<int, StageData> stageData = new();
    private readonly Dictionary<int, WaveData> waveData = new();
    private readonly Dictionary<int, MonsterSpawnGroupData> monsterSpawnGroupData = new();
    private readonly Dictionary<int, TileData> tileData = new();
    private readonly Dictionary<int, MonsterData> monsterData = new();
    private readonly Dictionary<int, RewardData> rewardData = new();
    private readonly Dictionary<int, RewardGroupData> rewardGroupData = new();
    private readonly Dictionary<int, TowerBaseData> towerBaseData = new();
    private readonly Dictionary<int, GachaData> gachaData = new();
    private readonly Dictionary<int, DebuffData> debuffData = new();
    private readonly Dictionary<int, AugmentData> augmentData = new();
    private readonly Dictionary<string, LocalizationData> localizationData = new();

    public IReadOnlyDictionary<int, StageData> StageData => stageData;
    public IReadOnlyDictionary<int, WaveData> WaveData => waveData;
    public IReadOnlyDictionary<int, MonsterSpawnGroupData> MonsterSpawnGroupData => monsterSpawnGroupData;
    public IReadOnlyDictionary<int, TileData> TileData => tileData;
    public IReadOnlyDictionary<int, MonsterData> MonsterData => monsterData;
    public IReadOnlyDictionary<int, RewardData> RewardData => rewardData;
    public IReadOnlyDictionary<int, RewardGroupData> RewardGroupData => rewardGroupData;
    public IReadOnlyDictionary<int, TowerBaseData> TowerBaseData => towerBaseData;
    public IReadOnlyDictionary<int, GachaData> GachaData => gachaData;
    public IReadOnlyDictionary<int, DebuffData> DebuffData => debuffData;
    public IReadOnlyDictionary<int, AugmentData> AugmentData => augmentData;
    public IReadOnlyDictionary<string, LocalizationData> LocalizationData => localizationData;
    #endregion

    private void DataParse()
    {

        // 스테이지 및 웨이브
        AddToDictionary(stageData, DataParser.Parse<StageData>("stage"), d => d.Index);
        AddToDictionary(waveData, DataParser.Parse<WaveData>("wave"), d => d.Index);

        // 몬스터 관련
        AddToDictionary(monsterSpawnGroupData, DataParser.Parse<MonsterSpawnGroupData>("mon_spawn_groups"), d => d.Index);
        AddToDictionary(monsterData, DataParser.Parse<MonsterData>("monsters"), d => d.MonsterId);

        // 타일 및 환경
        AddToDictionary(tileData, DataParser.Parse<TileData>("tiles"), d => d.Index);

        // 보상 관련
        AddToDictionary(rewardData, DataParser.Parse<RewardData>("rewards"), d => d.RewardId);
        AddToDictionary(rewardGroupData, DataParser.Parse<RewardGroupData>("reward_groups"), d => d.Index);

        // 타워 및 전투
        AddToDictionary(towerBaseData, DataParser.Parse<TowerBaseData>("tower_base"), d => d.TowerID);
        AddToDictionary(gachaData, DataParser.Parse<GachaData>("gacha"), d => d.Index);
        AddToDictionary(debuffData, DataParser.Parse<DebuffData>("debuff"), d => d.DebuffId);

        // 증강
        AddToDictionary(augmentData, DataParser.Parse<AugmentData>("augment"), d=> d.Index);

        AddToDictionary(localizationData, DataParser.Parse<LocalizationData>("localization"), d=> d.Index);

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
