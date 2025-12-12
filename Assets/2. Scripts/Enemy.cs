using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 적의 체력
    public float Health = 100f;

    // 타워 투사체 등으로부터 데미지를 받을 때 호출됨
    public void TakeDamage(float damage)
    {
        Health -= damage;

        // 체력이 0 이하가 되면 제거
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
