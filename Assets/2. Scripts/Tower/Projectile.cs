using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;        // 발사 대상
    public float speed = 10f;    // 이동 속도

    // 타겟 설정
    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // 타겟 없으면 제거
            return;
        }

        // 타겟 방향으로 이동
        Vector3 dir = (target.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        // 타겟 근접 시 제거 (체력 없음)
        if (Vector3.Distance(transform.position, target.transform.position) < 0.2f)
        {
            Destroy(gameObject);
        }
    }
}
