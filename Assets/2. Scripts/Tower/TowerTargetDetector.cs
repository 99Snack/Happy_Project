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

        // 리스트 역순 순회 (제거 중 발생할 수 있는 오류 방지 및 성능)
        //for (int i = enemies.Count - 1; i >= 0; i--)
        foreach(var enemy in enemies)
        {
            //Monster enemy = enemies[i];
            if (enemy == null || !enemy.gameObject.activeInHierarchy) continue;

            // 월드 좌표를 정수 좌표로 변환 (타일 기반)
            Vector3 pos = enemy.transform.position;
            int ex = Mathf.RoundToInt(pos.x);
            int ey = Mathf.RoundToInt(pos.z);

            // Chebyshev 거리 (타워 사거리 체크)
            int dx = Mathf.Abs(ex - towerTile.x);
            int dy = Mathf.Abs(ey - towerTile.y);

            if (dx <= range && dy <= range)
            {
                bestTarget = enemy;
                //// Manhattan 거리 (본진과의 거리 - 가장 멀리 온 적 우선 타겟팅 시 유리)
                //// 본진에 가장 가까운 적을 찾으려면 현재 로직(min)이 맞습니다.
                //int baseDistance = Mathf.Abs(ex - baseCampTile.x) + Mathf.Abs(ey - baseCampTile.y);

                //if (baseDistance < minBaseDistance)
                //{
                //    minBaseDistance = baseDistance;
                //    bestTarget = enemy;
                //}
                //break;
            }
        }
        return bestTarget;
    }
}