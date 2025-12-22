using UnityEngine;

public class TowerShooter : MonoBehaviour
{
    public GameObject projectilePrefab; // 총알 Prefab
    public Transform firePoint;         // 발사 위치

    // 총알 발사
    public void Shoot(MonsterMove target, float attackPower, int hitCount)
    {
        projectilePrefab = Resources.Load<GameObject>($"Prefab/Projectile");

        if (target == null) return;

        //GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        //todo : 임시 코드
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        Projectile projectile = proj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetTarget(target.transform); // 타겟 설정
        }

        // hitCount > 1이면 범위 공격 확장 가능
    }
}
