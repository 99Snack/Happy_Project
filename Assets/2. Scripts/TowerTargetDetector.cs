using UnityEngine;

public class TowerTargetDetector : MonoBehaviour
{
    private Enemy[] allEnemies; // 씬에 있는 모든 Enemy

    void Start()
    {
        // 씬의 모든 Enemy 자동 수집
        allEnemies = FindObjectsOfType<Enemy>();
        Debug.Log("Enemies found: " + allEnemies.Length);
    }

    // 기준 위치(referencePoint, BaseCamp)에서 가장 가까운 적 반환
    public Enemy FindNearestEnemy(Vector3 referencePoint)
    {
        Enemy nearest = null;
        float minDist = Mathf.Infinity;

        foreach (Enemy enemy in allEnemies)
        {
            if (enemy == null) continue;

            float dist = Vector3.Distance(referencePoint, enemy.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }

        return nearest;
    }
}
