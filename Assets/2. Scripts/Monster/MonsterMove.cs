using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    public float MoveSpeed = 3f; // 이동 속도
    public float AttackRange = 1f; // 공격 범위

    Transform targetAnchor; // 타겟 앵커 위치
    Animator animator; // 애니메이터 컴포넌트
   

    
    void Start()
    {
        animator = GetComponent<Animator>(); // 애니메이터 컴포넌트 가져오기
        targetAnchor = GameObject.FindWithTag("TargetAnchor").transform; // 타겟 앵커 태그 찾기
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q)) { OnHit(); } // 테스트용 
        if (Input.GetKeyDown(KeyCode.E)) { Attack(); }
        if (Input.GetKeyDown(KeyCode.W)) { Dead(); }

        float distance = Vector3.Distance(transform.position, targetAnchor.position); // 타겟과의 거리 계산

        if (distance > AttackRange) // 공격 범위 밖일 때 
        {
            Vector3 direction = (targetAnchor.position - transform.position).normalized; // 방향 벡터 계산
            if (direction != Vector3.zero) // 방향이 0이 아닐 때만 회전  // 플레이어가 이동방향으로 바라보게 함
            {
                transform.rotation = Quaternion.LookRotation(direction); // 방향 회전
            }

            transform.position = Vector3.MoveTowards(transform.position, targetAnchor.position, MoveSpeed * Time.deltaTime); // 이동
        }
        else // 도착하고 공격 범위 안일때 공격
        {
            Attack();
        }
    }

    public void SetTargetPosition(Vector3 newPosition) // 타겟 위치 설정 메서드
    {
        targetAnchor.position = newPosition; // 타겟 위치 설정
    }

    public void OnHit() // 타워 공격을 맞을 때 피격과 상호작용 
    { 
        animator.SetTrigger("Hit");
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
    }
    void Dead()
    {
        animator.SetTrigger("Die");
    }

}