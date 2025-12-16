using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    Monster monster;        // 몬스터 스크립트 참조
    Animator animator;      // 애니메이터 컴포넌트

    [Header("타겟 설정")]
    public Transform TargetAnchor; // 베이스캠프 도착지 타겟 위치 (인스펙터에서 설정 가능)

    public float AttackRange = 1f; // 공격 범위 (DB에 없어서 여기서 선언함)

    [Header("회전 감지 설정")]
    public float TurnAngleThreshold = 45f; // 이 각도 이상 회전하면 회전 애니메이션 재생
    
    
    private Vector3 _previousDirection; // 이전 프레임의 회전감지 하는 이동 방향 저장용 
    private bool _isTurning = false; // 회전 애니메이션 재생중인지 체크, 회전 중인지 여부 (중복 재생 방지용)

    void Start()
    {
        monster = GetComponent<Monster>();    // 몬스터 스크립트 가져오기
        animator = GetComponent<Animator>();  // 애니메이터 컴포넌트 가져오기

        _previousDirection = transform.forward; // 초기 이동 방향 설정
    }

    void Update()
    {
        if (TargetAnchor == null) return; // 따라갈 타겟이 없으면 종료

        if (Input.GetKeyDown(KeyCode.Q)) { OnHit(); } // 테스트용 
        if (Input.GetKeyDown(KeyCode.E)) { Attack(); }
        if (Input.GetKeyDown(KeyCode.W)) { Dead(); }

        float distance = Vector3.Distance(transform.position, TargetAnchor.position); // 타겟과의 거리 계산

        if (distance > AttackRange) // 공격 범위 밖일 때 
        {
            Vector3 direction = (TargetAnchor.position - transform.position).normalized; // 방향 벡터 계산
          
            if (direction != Vector3.zero) // 방향이 0이 아닐 때만 회전  
            {
                CheckTurnAnimation(direction); // 회전 애니메이션 체크 및 재생
                transform.rotation = Quaternion.LookRotation(direction); // 몬스터가 이동방향으로 바라보게 함 방향 회전
            }

            transform.position = Vector3.MoveTowards(transform.position, TargetAnchor.position, monster.Data.MoveSpeed * Time.deltaTime); // 이동
        
            _previousDirection = direction; // 현재 방향 저장 (다음 프레임 비교용) 
        }
        else // 도착하고 공격 범위 안일때 공격
        {
            Attack();
        }
    }


    /// <summary>
    /// 이전 방향과 현재 방향을 비교해서 회전 애니메이션 재생 
    /// </summary>
    void CheckTurnAnimation(Vector3 currentDirection)
    {
        // 이미 회전 중이면 스킵
        if (_isTurning) return;

        // 이전 방향과 현재 방향 사이의 각도 계산 (Y축 기준)
        float angle = Vector3.SignedAngle(_previousDirection, currentDirection, Vector3.up);

        // 설정한 각도 이상 회전했을때만 애니메이션 재생 
        if (Mathf.Abs(angle) >= TurnAngleThreshold)
        {
           if (angle > 0) // 양수 = 우회전 
            {
                animator.SetTrigger("TurnRight"); // 오른쪽 회전 애니메이션 재생
            }
            else // 음수 = 좌회전
            {
                animator.SetTrigger("TurnLeft"); // 왼쪽 회전 애니메이션 재생
            }

            _isTurning = true; // 회전 중 상태로 설정   

            // 회전 애니메이션 끝나면 다시 감지되게 
            Invoke("ResetTurnFlag", 0.5f); 
        }

    }

    // 회전 플래그 초기화 (Invoke로 호출)
    void ResetTurnFlag()
    {
        _isTurning = false; // 회전 중 상태 해제
    }

    // 스폰할 때 타겟 설정 (SpawnManager에서 호출)
    public void SetTarget(Transform target) // 타겟 위치 설정 메서드
    {
        TargetAnchor = target;
    }

    // 타워 공격을 맞을 때 피격과 상호작용 
    public void OnHit()
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