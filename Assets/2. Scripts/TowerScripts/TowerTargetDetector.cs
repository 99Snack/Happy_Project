using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerTargetDetector : MonoBehaviour
{
    public Tilemap tilemap;   // Inspector 연결

    private List<Enemy> enemies = new List<Enemy>();

    // 적 등록
    public void RegisterEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
            //Debug.Log($"[감지기] 적 등록됨 | 현재 적 수: {enemies.Count}");
        }
    }

    // 적 제거
    public void UnregisterEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
        //Debug.Log($"[감지기] 적 제거됨 | 현재 적 수: {enemies.Count}");
    }

  
    public Enemy FindNearestEnemyInRange(
        Vector3Int towerTile,
        int range,
        Vector3Int baseCampTile)
    {
        Enemy bestTarget = null;
        int minBaseDistance = int.MaxValue;

        //Debug.Log($"[감지기] 사거리 내 적 탐색 시작 | 전체 적 수: {enemies.Count}");

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null) continue;

            Vector3Int enemyTile = tilemap.WorldToCell(enemy.transform.position);

            // 타워 기준 사거리 검사 (Chebyshev)
            int dx = Mathf.Abs(enemyTile.x - towerTile.x);
            int dy = Mathf.Abs(enemyTile.y - towerTile.y);
            int towerDistance = Mathf.Max(dx, dy);

            if (towerDistance > range)
            {
                //Debug.Log($"[감지기] 사거리 OUT | 적({enemyTile.x},{enemyTile.y})");
                continue;
            }

            // BaseCamp 기준 거리
            int baseDistance =
                Mathf.Abs(enemyTile.x - baseCampTile.x) +
                Mathf.Abs(enemyTile.y - baseCampTile.y);

            //Debug.Log(
            //    $"[감지기] 사거리 IN | 적({enemyTile.x},{enemyTile.y}) " +
            //    $"BaseCamp 거리={baseDistance}"
            //);

            // 우선순위 비교
            if (baseDistance < minBaseDistance)
            {
                minBaseDistance = baseDistance;
                bestTarget = enemy;
            }
        }

        return bestTarget;
    }
}
