using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AugmentTag { Common = 0, Melee = 1, Range = 2, Single = 3, Area = 4, Debuff = 5 }

public class AugmentManager : MonoBehaviour
{
    private static AugmentManager instance;

    public static AugmentManager Instance { get => instance; set => instance = value; }

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

    public List<AugmentData> activeAugments = new List<AugmentData>();

    private const float DEFAULT_WEIGHT = 10;
    private const float BONUS_WEIGHT = 5f;

    private void Start()
    {
        //GetAugmentByTier(1);
        //GetAugmentByTier(2);
        //GetAugmentByTier(3);
    }

    public void ActivateAugment(AugmentData augment)
    {

        if (activeAugments.Contains(augment)) return;

        activeAugments.Add(augment);

        //Debug.Log($"{activeAugments.Count} :{augment}");
      
        ApplyAugmentToAllTowers(augment);
    }

    private void ApplyAugmentToAllTowers(AugmentData augment)
    {
        if (TowerManager.Instance == null) return;

        foreach (var tower in TowerManager.Instance.allTowers)
        {
            if (tower != null)
            {
                if (augment.Category == 3)
                {
                    tower.AddConditionAugment(augment);
                }

                tower.ApplyAugment(augment);
                Debug.Log(DataManager.Instance.LocalizationData[augment.Name_STR]);
            }
        }
    }

    public void ApplyAllActiveAugmentsToTower(Tower newTower)
    {
        foreach (var augment in activeAugments)
        {
            newTower.ApplyAugment(augment);
        }
    }

    public List<AugmentData> GetGeneratorRandomAugment(int wave = 1)
    {
        return GetAugmentByTier(wave);
    }
    private List<AugmentData> GetAugmentByTier(int wave)
    {
        int targetTier = wave switch
        {
            >= 9 => 3,
            >= 4 => 2,
            >= 1 => 1,
            _ => 1
        };

        //타워 분석 및 태그별 계산
        var tagScores = CalculateTowerTagScores();

        //가중치 보정
        Dictionary<AugmentTag, float> weightBoosts = DetermineAddWeight(tagScores);

        //티어 일치 및 보유 중복 제
        var availablePool = DataManager.Instance.AugmentData.Values
            .Where(d => d.Tier == targetTier && !activeAugments.Contains(d))
            .ToList();

        if (availablePool.Count == 0) return new List<AugmentData>();

        //가중치 기반 랜덤 추출
        return PickMultipleWeightedRandom(availablePool, weightBoosts, 3);
    }

    //(1성=1, 2성=3, 3성=9)
    private Dictionary<AugmentTag, int> CalculateTowerTagScores()
    {
        Dictionary<AugmentTag, int> scores = new Dictionary<AugmentTag, int> {
            { AugmentTag.Melee, 0 }, { AugmentTag.Range, 0 },
            { AugmentTag.Single, 0 }, { AugmentTag.Area, 0 }, { AugmentTag.Debuff, 0 }
        };

        foreach (var tower in TowerManager.Instance.allTowers)
        {
            int score = tower.Data.Grade switch
            {
                1 => 1,
                2 => 3,
                3 => 9,
                _ => 1
            };

            //근거리/원거리 분류
            if (tower is MeleeTower) scores[AugmentTag.Melee] += score;
            else if (tower is RangeTower) scores[AugmentTag.Range] += score;

            //상세 타입 분류
            if (tower is SpearTower || tower is MineThrowerTower) scores[AugmentTag.Single] += score;
            if (tower is KnightTower || tower is MageTower) scores[AugmentTag.Area] += score;
            if (tower is BloodKnightTower || tower is IceMageTower) scores[AugmentTag.Debuff] += score;
        }
        return scores;
    }

    //조건에 따른 가중치 추가 로직
    private Dictionary<AugmentTag, float> DetermineAddWeight(Dictionary<AugmentTag, int> scores)
    {
        var boosts = new Dictionary<AugmentTag, float>();
        foreach (AugmentTag tag in System.Enum.GetValues(typeof(AugmentTag))) boosts[tag] = 0;

        //근거리 vs 원거리 비교
        if (scores[AugmentTag.Melee] > scores[AugmentTag.Range])
        {
            boosts[AugmentTag.Melee] += BONUS_WEIGHT;
        }
        else if (scores[AugmentTag.Range] > scores[AugmentTag.Melee])
        {
            boosts[AugmentTag.Range] += BONUS_WEIGHT;
        }
        else
        {
            boosts[AugmentTag.Melee] += BONUS_WEIGHT;
            boosts[AugmentTag.Range] += BONUS_WEIGHT;
        }

        //단일, 광역, 디버프 가중치 추가
        if (scores[AugmentTag.Single] >= 3)
        {
            boosts[AugmentTag.Single] += BONUS_WEIGHT;
        }
        if (scores[AugmentTag.Area] >= 3)
        {
            boosts[AugmentTag.Area] += BONUS_WEIGHT;
        }
        if (scores[AugmentTag.Debuff] >= 3)
        {
            boosts[AugmentTag.Debuff] += BONUS_WEIGHT;
        }

        return boosts;
    }

    //랜덤 추출
    private List<AugmentData> PickMultipleWeightedRandom(List<AugmentData> filterList, Dictionary<AugmentTag, float> boosts, int count)
    {
        List<AugmentData> selected = new List<AugmentData>();
        List<AugmentData> filter = new List<AugmentData>(filterList);

        for (int i = 0; i < count; i++)
        {
            if (filter.Count == 0) break;

            float totalWeight = filter.Sum(d => DEFAULT_WEIGHT + boosts[(AugmentTag)d.Tag]);
            float pivot = Random.Range(0f, totalWeight);
            float currentSum = 0;

            foreach (var item in filter)
            {
                currentSum += (DEFAULT_WEIGHT + boosts[(AugmentTag)item.Tag]);
                if (pivot <= currentSum)
                {
                    selected.Add(item);
                    filter.Remove(item); 
                    break;
                }
            }
        }
        return selected;
    }
}
