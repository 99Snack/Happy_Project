using System.Collections.Generic;
using UnityEngine;

public class TowerTargetDetector : MonoBehaviour
{
    private static TowerTargetDetector instance;
    public static TowerTargetDetector Instance => instance;

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

    private List<Monster> enemies = new List<Monster>();

    public void RegisterEnemy(Monster enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
            //Debug.Log($"[감지기] 적 등록됨 | 현재 적 수: {enemies.Count}");
        }
    }

    // 적 제거
    public void UnregisterEnemy(Monster enemy)
    {
        enemies.Remove(enemy);
        //Debug.Log($"[감지기] 적 제거됨 | 현재 적 수: {enemies.Count}");
    }

    public Monster FindNearestEnemyInRange(
        Vector2Int towerTile,
        int range,
        Vector2Int baseCampTile)
    {
        Monster bestTarget = null;
        int minBaseDistance = int.MaxValue;

        //Debug.Log($"[감지기] 사거리 내 적 탐색 시작 | 전체 적 수: {Monster.Count}");

        foreach (Monster enemy in enemies)
        {
            if (enemy == null || !enemy.gameObject.activeSelf) continue;

            int enemyTileX = Mathf.RoundToInt(enemy.transform.position.x);
            int enemyTileY = Mathf.RoundToInt(enemy.transform.position.z);

            Vector2Int enemyTile = new Vector2Int(enemyTileX, enemyTileY);


            // 타워 기준 사거리 검사 (Chebyshev)
            int dx = Mathf.Abs(enemyTileX - towerTile.x);
            int dy = Mathf.Abs(enemyTileY - towerTile.y);
            int towerDistance = Mathf.Max(dx, dy);

            //Debug.Log($"({towerTile}:{enemyTile}) | {towerDistance} <= {range}");

            if (towerDistance <= range)
            {
                // BaseCamp 기준 거리
                int baseDistance =
                    Mathf.Abs(enemyTileX - baseCampTile.x) +
                    Mathf.Abs(enemyTileY - baseCampTile.y);

                //Debug.Log(
                //    $"[감지기] 사거리 IN | 적({enemyTile.x},{enemyTile.y}) " +
                //    $"BaseCamp 거리={baseDistance}"
                //);

                // 우선순위 비교
                //Debug.Log($"{baseDistance} < {minBaseDistance}");
                if (baseDistance < minBaseDistance)
                {
                    minBaseDistance = baseDistance;
                    bestTarget = enemy;
                }
            }
        }

        return bestTarget;
    }
}