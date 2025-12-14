using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerTargetDetector : MonoBehaviour
{
    public Tilemap tilemap; // 기준이 되는 타일맵

    // BaseCamp 타일 좌표를 기준으로 가장 가까운 적 반환
    public Enemy FindNearestEnemy(Vector3Int baseCampTile)
    {
        // 현재 씬에 존재하는 모든 Enemy를 다시 수집
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        if (tilemap == null)
        {
            Debug.LogError("Tilemap is not assigned");
            return null;
        }

        Enemy nearest = null;
        int shortestDistance = int.MaxValue;

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null) continue;

            // 적 위치를 타일 좌표로 변환
            Vector3Int enemyTile =
                tilemap.WorldToCell(enemy.transform.position);

            // 3D 타일맵이므로 x z 방향 거리만 사용
            int tileDistance =
                Mathf.Abs(baseCampTile.x - enemyTile.x) +
                Mathf.Abs(baseCampTile.z - enemyTile.z);

            // BaseCamp에 가장 가까운 적 선택
            if (tileDistance < shortestDistance)
            {
                shortestDistance = tileDistance;
                nearest = enemy;
            }
        }

        return nearest;
    }
}
