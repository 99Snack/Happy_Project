using System.IO;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    Monster monster;        // 몬스터 스크립트 참조
    Animator animator;      // 애니메이터 컴포넌트

    [Header("타겟 설정")]
    public Vector3 TargetAnchor; // 베이스캠프 도착지 타겟 위치 (인스펙터에서 설정 가능)
    public float AttackRange = 1f; // 공격 범위 (DB에 없어서 여기서 선언함)

    // 방향 관련 
    Vector3 nextDir; // 회전 후 바라볼 방향 
    Vector3 currentLookDir; // 현재 바라보는 방향

    // 회전 관련 
    public bool isTurning = false;
    float turnProgress = 0f; // 회전 진행도 (0~1)
    float turnDuration = 0.5f; // 회전에 걸리는 시간

    Vector2Int[] path;
    int currentIdx = 1;

    private void Awake()
    {
        monster = GetComponent<Monster>();    // 몬스터 스크립트 가져오기
        animator = GetComponent<Animator>();  // 애니메이터 컴포넌트 가져오기
                                              // animator = transform.GetChild(0).GetComponent<Animator>(); // 몬스터 모델이 자식에 있을 때
    }

    void OnEnable()
    {
        currentLookDir = transform.forward; // 초기 방향 설정



        TowerTargetDetector.Instance.RegisterEnemy(this);

        path = SinglePathGenerator.Instance.GetCurrentPath();
        if (path != null && path.Length > 0)
        {
            TargetAnchor = TileManager.Instance.GetWorldPosition(path[0]);
            currentLookDir = (TargetAnchor - transform.position).normalized;
        }

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

        // 회전 처리
        if (isTurning)
        {
            turnProgress += Time.deltaTime / turnDuration;

            if (turnProgress >= 1f)
            {
                isTurning = false;
                currentLookDir = nextDir;
            }
        }

        /// 테스트용 웨이포인트 
        if (path != null && path.Length > 0)
        {
            if (currentIdx >= path.Length) return;
            TargetAnchor = TileManager.Instance.GetWorldPosition(path[currentIdx]);
            float dist = Vector3.Distance(transform.position, TargetAnchor);
            if (dist < 0.1f)
            {
                currentIdx++;

                if (currentIdx >= path.Length)
                {
                    currentIdx = 0; // 무한 반복 
                }

                TargetAnchor = TileManager.Instance.GetWorldPosition(path[currentIdx]);
                Vector3 nextWaypointDir = (TargetAnchor - transform.position).normalized;
                SetDirection(nextWaypointDir);  // 길찾기가 호출할 메서드
            }
        }
        /// 나중에 삭제 


        // 따라갈 타겟이 없으면 종료
        if (TargetAnchor == null) return;

        float distance = Vector3.Distance(transform.position, TargetAnchor); // 타겟과의 거리 계산

        if (distance > AttackRange)
        {
            // 이동 방향은 타겟을 향해
            Vector3 moveDir = (TargetAnchor - transform.position).normalized;
            transform.position += moveDir * monster.Data.MoveSpeed * Time.deltaTime;

            if (!isTurning)   // 회전 중이 아닐 때만 이동 방향을 바라봄
            {
                currentLookDir = moveDir;
            }
        }
        else // 도착하고 공격 범위 안일때 공격
        {
            Attack();
        }
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(currentLookDir); // 매 프레임 마지막에 현재 바라보는 방향으로 회전 적용 
    }

    public void SetDirection(Vector3 direction)
    {
        if (isTurning) return; // 이미 회전 중이면 무시

        float turnAngle = Vector3.SignedAngle(currentLookDir, direction, Vector3.up);
        if (turnAngle > 45f) // 방향 전환
        {
            TurnRight();
            StartTurn(direction);
        }
        else if (turnAngle < -45f)
        {
            TurnLeft();
            StartTurn(direction);
        }
        else  // 같은 방향이면 앞 바라보게 설정
        {
            currentLookDir = direction;
        }
    }

    // 회전 시작 메서드
    void StartTurn(Vector3 targetDirection)
    {
        isTurning = true;

        turnProgress = 0f; // 회전 진행도 초기화
        nextDir = targetDirection; // 회전 후 바라볼 목표 방향 설정
    }

    // 스폰할 때 타겟 설정 (SpawnManager에서 호출)
    public void SetTarget(Transform target) // 타겟 위치 설정 메서드
    {
        TargetAnchor = target.position;
    }

    // 총알 충돌 (총알 프리팹에 태그 불렛과, 콜라이더에 Is Trigger 체크 필요)
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet")) // 총알 태그와 충돌했을 때 // 
        {
            OnHit(); // 피격 애니메이션 호출 
            
            Projectile projectile = other.GetComponent<Projectile>();
            if (projectile != null)
            {
                monster.TakeDamage(projectile.damage); // 피 까임
            }

             
          //  Destroy(other.gameObject); // 총알 파괴
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


    void TurnLeft()
    {
        animator.SetTrigger("TurnLeft");
    }

    void TurnRight()
    {
        animator.SetTrigger("TurnRight");
    }
    void Spawn()
    {
        //스폰하는동안 이동못하게 코루틴 필요
        animator.SetTrigger("Spawn");
    }
}