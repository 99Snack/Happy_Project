using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 적의 체력
    public float Health = 100f;

    private TowerTargetDetector detector;

    void Start()
    {
        detector = FindFirstObjectByType<TowerTargetDetector>();

        //Debug.Log("[적] 생성됨");

        if (detector != null)
        {
            detector.RegisterEnemy(this);
            //Debug.Log("[적] 타워 타겟 감지기에 등록됨");
        }
        else
        {
            //Debug.LogError("[적] 타워 타겟 감지기를 찾을 수 없음!");
        }
    }


    void OnDestroy()
    {
        if (detector != null)
        {
            detector.UnregisterEnemy(this);
            //Debug.Log("[적] 제거됨 → 감지기에서 등록 해제");
        }
    }

    // 타워 투사체 등으로부터 데미지를 받을 때 호출됨
    public void TakeDamage(float damage)
    {
        Health -= damage;

        if (Health <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
