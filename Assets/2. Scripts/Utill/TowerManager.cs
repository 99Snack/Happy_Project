using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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

    public static readonly int GACHA_PRICE = 100;

    public List<TileInteractor> waitingSeat = new List<TileInteractor>();
    public List<Tower> allTowers = new List<Tower>();

    private bool isMerge = false;

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
                //Debug.Log(towerId);
                tower?.Setup(towerId, tile);

                allTowers.Add(tower);

                if (tower == null) return;

                if (tower.Data?.Grade != 3)
                {
                    //임시 제한
                    if (tower.Data.TowerID == 101001 ||
                    tower.Data.TowerID == 101002)
                    {
                        CheckUpgrade(tower);
                    }
                }

                return;
            }
        }

    }

    void CheckUpgrade(Tower tower)
    {
        if (tower.Data.Grade == 3) return;

        var matches = allTowers.Where(t => t.Data.TowerID == tower.Data.TowerID).ToList();

        if (matches.Count >= 3)
        {
            if (isMerge) return;

            var sortedMatches = matches
                                //1. 타일 위 여부
                                .OrderByDescending(t => t.MyTile.Type == TileInfo.TYPE.Wall)
                                //2. 먼저 배치된 순서
                                .ThenBy(t => t.PlacedTime)
                                .ToList();

            TowerUpgrade(sortedMatches);
        }
    }

    void TowerUpgrade(List<Tower> towers)
    {
        //기준 타워    
        Tower standardTower = towers[0];

        List<Tower> removeTowers = new List<Tower>();
        for (int i = 1; i < 3; i++)
        {
            removeTowers.Add(towers[i]);

            //정보 창이 열려있다면 닫기
            if (towers[i] == UIManager.Instance.CurrentTower)
            {
                UIManager.Instance.CloseTowerInfo();
            }

            allTowers.Remove(towers[i]);
        }

        StartCoroutine(MergeEffect(standardTower, removeTowers));
    }

    IEnumerator MergeEffect(Tower target, List<Tower> removeTowers)
    {
        Time.timeScale = 0;
        isMerge = true;

        yield return new WaitForSecondsRealtime(0.3f);

        Vector3 targetPos = target.transform.position;

        List<Vector3> startPos = new List<Vector3>();
        foreach (var remove in removeTowers)
        {
            startPos.Add(remove.transform.position);
        }

        float timer = 0f;
        float duration = 0.5f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / duration;

            float curveT = t * t;

            for (int i = 0; i < removeTowers.Count; i++)
            {
                if (removeTowers[i] != null)
                {
                    removeTowers[i].transform.position = Vector3.Lerp(startPos[i], targetPos, curveT);
                    removeTowers[i].transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
                }
            }
            yield return null;
        }

        foreach (var remove in removeTowers)
        {
            if (remove != null)
            {
                remove.MyTile.isAlreadyTower = false;

                Destroy(remove.gameObject);
            }
        }

        //성급 이펙트 구현
        //if(gradeEffect !=null){
        //    Instantiate(gradeEffect, targetPos + Vector3.up * 0.5f, Quaternion.identity);
        //}

        target.Upgrade();

        Time.timeScale = 1f;
        isMerge = false;
        CheckUpgrade(target);
    }

    GameObject SelectPrefab(int towerId)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefab/{towerId}");

        if (prefab == null)
        {
            Debug.LogError($"{towerId}에 해당하는 타워가 없습니다.");
        }

        return prefab;
    }

    public void SellTower(Tower sellTower)
    {
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
