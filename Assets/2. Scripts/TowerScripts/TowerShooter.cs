using UnityEngine;

public class TowerShooter : MonoBehaviour
{
    public GameObject projectilePrefab; // 총알 Prefab
    public Transform firePoint;         // 발사 위치

    // 총알 발사
    public void Shoot(Enemy target, float attackPower, int hitCount)
    {
        if (target == null) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectile = proj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetTarget(target.transform); // 타겟 설정
        }

        // hitCount > 1이면 범위 공격 확장 가능
    }

    public void Shoot(MoveTest target, float attackPower, int hitCount)
    {
        if (target == null) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectile = proj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetTarget(target.transform); // 타겟 설정
        }

        // hitCount > 1이면 범위 공격 확장 가능
    }
}
