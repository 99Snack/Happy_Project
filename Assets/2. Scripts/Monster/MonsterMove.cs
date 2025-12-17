using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    Monster monster;        // 몬스터 스크립트 참조
    Animator animator;      // 애니메이터 컴포넌트

    [Header("타겟 설정")]
    public Transform TargetAnchor; // 베이스캠프 도착지 타겟 위치 (인스펙터에서 설정 가능)

    public float AttackRange = 1f; // 공격 범위 (DB에 없어서 여기서 선언함)

    void Start()
    {
        monster = GetComponent<Monster>();    // 몬스터 스크립트 가져오기
        animator = GetComponent<Animator>();  // 애니메이터 컴포넌트 가져오기

        Spawn();  // 스폰 될때 스폰 애니 재생 
    }

    void Update()
    {
        // 테스트용 
        if (Input.GetKeyDown(KeyCode.Alpha1)) { OnHit(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { Attack(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { Dead(); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { TurnLeft(); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { TurnRight(); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { Spawn(); }

        // 따라갈 타겟이 없으면 종료
        if (TargetAnchor == null) return; 

        float distance = Vector3.Distance(transform.position, TargetAnchor.position); // 타겟과의 거리 계산

        if (distance > AttackRange) // 공격 범위 밖일 때 
        {
            Vector3 direction = (TargetAnchor.position - transform.position).normalized; // 방향 벡터 계산

            if (direction != Vector3.zero) // 방향이 0이 아닐 때만 회전  
            {
                transform.rotation = Quaternion.LookRotation(direction); // 몬스터가 이동방향으로 바라보게 함 방향 회전
            }

            transform.position = Vector3.MoveTowards(transform.position, TargetAnchor.position, monster.Data.MoveSpeed * Time.deltaTime); // 이동

        }
        else // 도착하고 공격 범위 안일때 공격
        {
            Attack();
        }
    }

    // 스폰할 때 타겟 설정 (SpawnManager에서 호출)
    public void SetTarget(Transform target) // 타겟 위치 설정 메서드
    {
        TargetAnchor = target;
    }

    // 테스트용 총알 충돌 (총알 프리팹에 태그 불렛과, 콜라이더에 Is Trigger 체크 필요)
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bullet")) // 총알 태그와 충돌했을 때 // 
        {
            OnHit(); // 피격 애니메이션 호출 
           // Destroy(other.gameObject); // 총알 파괴
        }

        // 나중에 hp도 까이게 
    }
   
    public void OnHit() 
    {
        animator.SetTrigger("Hit");   // 피격 애니메이션 다 출력되면 다시 이동으로 바뀜. 다른 메서드도 동일 

        // 타워 공격을 맞을 때 피격과 상호작용
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
        
        // 아마 여기서 베이스캠프를 공격하면 피해를 입히게 할 듯
    }
    void Dead()
    {
        animator.SetTrigger("Die");

        // 죽으면 이동도 안되게 해야 함 
    }

    // 길찾기에서 호출할 회전 메서드 
    void TurnLeft() 
    {
        animator.SetTrigger("TurnLeft");

        // 적이 앞만 바라보고 가는 구조라 회전을 여기서 확인할 수가 없음 
        // 그냥 길찾기에서 회전이 필요할 때 이 메서드를 호출하게 하는 게 편할듯 
    }

    void TurnRight()
    {
        animator.SetTrigger("TurnRight");
    }
    void Spawn()
    {
        animator.SetTrigger("Spawn");
    }
}