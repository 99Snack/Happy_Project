using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    private static TowerManager instance;

    public static TowerManager Instance { get => instance; set => instance = value; }

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

    #region 타워 프리팹
    public GameObject spearTower;
    public GameObject MineThrowerTower;
    #endregion

    public List<TileInteractor> waitingSeat = new List<TileInteractor>();

    public int Gacha()
    {
        int waitTowerCount = waitingSeat.Count(t => t.isAlreadyTower == true);

        if (waitTowerCount >= waitingSeat.Count)
        {
            Debug.Log($"대기석이 꽉 찼습니다. 대기석에 있는 타워 수 : {waitTowerCount}");
            return -1;
        }

        int basicIdx = DataManager.Instance.GachaData.Keys.ElementAt(0);
        int towerId = DataManager.Instance.GachaData[basicIdx].TowerID;

        //0 ~ 1.0 랜덤값 뽑음
        float randomValue = Random.value;
        float cumulative = 0;

        foreach (var gacha in DataManager.Instance.GachaData)
        {
            cumulative += gacha.Value.Probability;
            if (randomValue < cumulative)
            {
                towerId = gacha.Value.TowerID;
                break;
            }
        }

        GeneratorTower(towerId);

        return towerId;
    }
    
    void GeneratorTower(int towerId)
    {
        GameObject prefab = SelectPrefab(towerId);

        foreach (var tile in waitingSeat)
        {
            if (!tile.isAlreadyTower)
            {
                GameObject tower = Instantiate(prefab, tile.transform);
                tower.transform.localPosition = new Vector3(0, 0.5f, 0);
                tile.isAlreadyTower = true;
                return;
            }
        }

        Tower_Base data = DataManager.Instance.TowerBaseData[towerId];
    }

    //todo : 추후 타워 id말고 타워 이름(string)으로 리소스에서 프리팹 가져와 생성하는 식으로
    GameObject SelectPrefab(int towerId)
    {
        GameObject prefab = null;

        switch (towerId)
        {
            case 10101:
                break;
            default:
                prefab = MineThrowerTower;
                break;
        }
        return prefab;
    }
}
