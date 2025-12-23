using UnityEngine;
using System.Collections;

public class MonsterMove : MonoBehaviour
{
    Monster monster;        // 몬스터 스크립트 참조

    [Header("타겟 설정")]
    public Vector3 TargetAnchor; // 베이스캠프 도착지 타겟 위치 (인스펙터에서 설정 가능)
    [Header("공격 범위")]
    public float AttackRange = 1f; // 공격 범위 (DB에 없어서 여기서 선언함)

    [Header("막다른 길 감지 거리 설정")]
    [SerializeField] int deadEndMoveLimit = 2; // 막다른 길에서 뒤로 물러날 거리

    [Header("피드백 UI 프리팹")]
    public GameObject FeedbackUIPrefab; // 피드백 UI 프리팹 (피격 시 느낌표!)

    // 방향 관련 
    Vector3 nextDir; // 회전 후 바라볼 방향 
    Vector3 currentLookDir; // 현재 바라보는 방향

    // 회전 관련 
    public bool isTurning = false;
    float turnProgress = 0f; // 회전 진행도 (0~1)
    float turnDuration = 0.5f; // 회전에 걸리는 시간

    // 경로 관련 
    Vector2Int[] path; // 실제 이동 경로
    Vector2Int[] feedbackPoints;  // 피드백 출력할 위치 지점
    int currentIdx = 0; // 현재 경로 인덱스 

    // 피드백 관련
    bool isFeedbackPaused = false; // 피드백 출력 중 일시정지 상태

    private void Awake()
    {
        monster = GetComponent<Monster>();    // 몬스터 스크립트 가져오기
    }

    void OnEnable()
    {
        currentLookDir = transform.forward; // 초기 방향 설정
    }

    void Start()
    {
        // PathNodeManager에서 경로 및 피드백 좌표 지점 가져오기
        int spawnNum = monster.GetSpawnNumber(); // 몬스터의 스폰 번호 가져오기

        if (PathNodeManager.Instance != null)
        {
            path = PathNodeManager.Instance.GetPathNode(spawnNum, deadEndMoveLimit, out feedbackPoints);
            Debug.Log("몬스터 스폰 번호: " + spawnNum + ", 경로 길이: " + path.Length + ", 피드백 좌표 개수: " + feedbackPoints.Length);
        }

        if (path != null && path.Length > 0)
        {
            currentIdx = 0;
            TargetAnchor = TileManager.Instance.GetWorldPosition(path[0]);
            currentLookDir = (TargetAnchor - transform.position).normalized;
        }

        monster.Spawn();  // 스폰 될때 스폰 애니 재생 
    }

    void Update()
    {
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

        // 피드백: 일시정지 상태면 이동 안 함
        if (isFeedbackPaused) return;


        // 경로 따라 이동, 없으면 종료
        if (path != null && path.Length > 0)
        {
            if (currentIdx >= path.Length) return;

            // 현재 타겟 위치 
            TargetAnchor = TileManager.Instance.GetWorldPosition(path[currentIdx]);
            float dist = Vector3.Distance(transform.position, TargetAnchor);

            // 피드백 위치 도달 체크
            if (dist < 0.1f)
            {
                // 피드백 지점인지 체크
                if (CheckFeedbackPoint(path[currentIdx]))
                {
                    // 피드백 출력 코루틴 실행
                    StartCoroutine(DoFeedbackAction());
                    return; // 피드백 중에는 이동 멈춤
                }
                currentIdx++; // 다음 경로 인덱스로 이동

                // 목적지 도착
                if (currentIdx >= path.Length)
                {
                    monster.Attack(); // 도착 시 공격  // 단일 공격 
                    return; // 더 이상 이동할 경로가 없으면 종료
                }

                // 다음으로 갈 곳 방향 재설정 갱신 
                TargetAnchor = TargetAnchor = TileManager.Instance.GetWorldPosition(path[currentIdx]);
                Vector3 nextWaypointDir = (TargetAnchor - transform.position).normalized;
                SetDirection(nextWaypointDir); // 방향 전환 
            }
        }

        // 따라갈 타겟이 없으면 종료 
        if (TargetAnchor == null) return;

        // 이동 
        float distance = Vector3.Distance(transform.position, TargetAnchor); // 타겟과의 거리 계산
        if (distance > AttackRange)
        {
            // 이동 방향은 타겟을 향해
            Vector3 moveDir = (TargetAnchor - transform.position).normalized;
            //todo : transform.position += moveDir * monster.Data.MoveSpeed * Time.deltaTime;
            transform.position += moveDir * 2f * Time.deltaTime;

            if (!isTurning)   // 회전 중이 아닐 때만 이동 방향을 바라봄
            {
                currentLookDir = moveDir;
            }
        }
        else // 도착하고 공격 범위 안일때 공격
        {
            monster.Attack(); // 계속 공격 
        }
    }

    void LateUpdate()
    {
        // 매 프레임 마지막에 현재 바라보는 방향으로 회전 적용 
        transform.rotation = Quaternion.LookRotation(currentLookDir);
    }

    // 피드백 지점인지 체크
    bool CheckFeedbackPoint(Vector2Int currentPos)
    {
        if (feedbackPoints == null) return false;

        foreach (var point in feedbackPoints)
        {
            if (point == currentPos)
            {
                return true;
            }
        }
        return false;
    }

    // 피드백 출력 코루틴
    IEnumerator DoFeedbackAction()
    {
        isFeedbackPaused = true;

        // 머리 위에 피드백 ui 생성 
        GameObject feedbackUI = null;
        if (FeedbackUIPrefab != null)
        {
            Vector3 headPos = transform.position + Vector3.up * 2f; // 머리 위 높이 조절
            feedbackUI = Instantiate(FeedbackUIPrefab, headPos, Quaternion.identity, transform);
        }

        // 피드백 출력 중 일시정지 상태 1초
        yield return new WaitForSeconds(1f);
        // 피드백 UI 제거
        if (feedbackUI != null)
        {
            Destroy(feedbackUI);
        }
        // 첫번째 90도 회전
        Vector3 firstTurnDir = Quaternion.Euler(0, 90, 0) * currentLookDir;
        monster.TurnRight();
        StartTurn(firstTurnDir);
        yield return new WaitForSeconds(0.5f); // 0.5초 대기 = 회전 애니메이션과 같음

        // 두번째 180도 회전 
        Vector3 secondTurnDir = Quaternion.Euler(0, 90, 0) * firstTurnDir;
        monster.TurnRight();
        StartTurn(secondTurnDir);
        yield return new WaitForSeconds(0.5f);


        currentIdx++; // 다음 경로 인덱스로 이동
        isFeedbackPaused = false;
    }

    public void SetDirection(Vector3 direction)
    {
        if (isTurning) return; // 이미 회전 중이면 무시

        float turnAngle = Vector3.SignedAngle(currentLookDir, direction, Vector3.up);
        if (turnAngle > 45f) // 방향 전환
        {
            monster.TurnRight();
            StartTurn(direction);
        }
        else if (turnAngle < -45f)
        {
            monster.TurnLeft();
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
}