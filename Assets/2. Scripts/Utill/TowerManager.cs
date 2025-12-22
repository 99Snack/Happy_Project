using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct TowerPrefabData{
    public int towerId;
    public GameObject prefab;
}

public class TowerManager : MonoBehaviour
{
    private static TowerManager instance;

    public static TowerManager Instance { get => instance; set => instance = value; }

    #region 타워 프리팹
    public GameObject spearTower;
    public GameObject MineThrowerTower;
    public List<TowerPrefabData> resources = new List<TowerPrefabData>();
    private Dictionary<int, GameObject> towerPrefabDic = new Dictionary<int, GameObject>();
    #endregion

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

        foreach(var resource in resources)
        {
            towerPrefabDic.Add(resource.towerId, resource.prefab);
        }
    }

   

    public List<TileInteractor> waitingSeat = new List<TileInteractor>();
    public List<Tower> allTowers = new List<Tower>();

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
                GameObject towerObj = Instantiate(prefab, tile.transform);
                towerObj.transform.localPosition = new Vector3(0, 0.5f, 0);
                tile.isAlreadyTower = true;

                Tower tower = towerObj.transform.GetComponent<Tower>();
                tower.Setup(towerId, tile);

                allTowers.Add(tower);
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

    public void SellTower(Tower sellTower){
        //1. 리스트에서 제거
        allTowers.Remove(sellTower);
        //2. 판매한 재화 게임매니저에서 제어
        //3. 타일 isAlreadyTower =false 변경
        sellTower.MyTile.isAlreadyTower = false;
        //TileInteractor interactor = sellTower.transform.parent.GetComponent<TileInteractor>();
        //interactor.isAlreadyTower = false;

        //4. currentTower= null 변경
        //5. 타워 객체에도 Sell메서드 실행
        sellTower.OnSold();
    }
}
