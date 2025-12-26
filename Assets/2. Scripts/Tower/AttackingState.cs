using System.Collections;
using UnityEngine;

public class AttackingState : ITowerState
{
    private Tower tower;
    private float nextAttackTime = 0f;
    private bool isFirstAttack = true;
    private float currentAnimLength = 0f;

    public AttackingState(Tower tower)
    {
        this.tower = tower;
    }

    public void Enter()
    {
        tower.animator.SetBool(tower.hashIsAttacking, true);
        isFirstAttack = true;
        nextAttackTime = 0f;
    }

    public void Update()
    {
        // 타겟 유효성 검사
        if (tower.currentTarget == null || !tower.currentTarget.gameObject.activeInHierarchy)
        {
            tower.currentTarget = null;
            tower.ChangeState(tower.SearchingState);
            return;
        }

        // 사거리 체크
        if (!tower.IsTargetInRange())
        {
            tower.ChangeState(tower.SearchingState);
            return;
        }

        // 회전 처리 (방향 수정)
        if (tower.IsRotate)
        {
            Vector3 direction = tower.Soldier.position- tower.currentTarget.transform.position;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion lookTarget = Quaternion.LookRotation(direction);
                tower.Soldier.rotation = Quaternion.Slerp(
                    tower.Soldier.rotation,
                    lookTarget,
                    Time.deltaTime * 10f
                );
            }
        }

        // 공격 타이밍 체크
        if (Time.time >= nextAttackTime)
        {
            if (tower.currentTarget != null)
            {
                PerformAttack();
            }
        }
    }

    private void PerformAttack()
    {
        // 애니메이션 트리거
        tower.animator.SetTrigger(tower.hashAttack);

        // 현재 재생될 애니메이션 길이 가져오기
        tower.StartCoroutine(AdjustAnimationSpeed());

        // 다음 공격 시간 설정 (인터벌 보장)
        nextAttackTime = Time.time + tower.Data.AttackInterval;

        isFirstAttack = false;
    }

     IEnumerator AdjustAnimationSpeed()
    {
        // 트리거 발동 후 애니메이션이 실제로 시작될 때까지 대기
        yield return null;

        AnimatorStateInfo stateInfo = tower.animator.GetCurrentAnimatorStateInfo(0);

        // 공격 애니메이션 State 확인 (Tag나 이름으로)
        if (stateInfo.IsTag("Attack"))
        {
            currentAnimLength = stateInfo.length;

            if (currentAnimLength > 0)
            {
                // 애니메이션 속도 = 애니메이션 원본 길이 / 원하는 인터벌
                // 예: 애니메이션이 2초, 인터벌이 1초면 speed = 2 (2배속)
                // 예: 애니메이션이 0.5초, 인터벌이 1초면 speed = 0.5 (0.5배속)
                float calculatedSpeed = currentAnimLength / tower.Data.AttackInterval;
                tower.animator.SetFloat(tower.hashAttackInterval, calculatedSpeed);
            }
        }
    }

    public void Exit()
    {
        tower.animator.SetBool(tower.hashIsAttacking, false);
        // 애니메이션 속도 초기화
        tower.animator.SetFloat(tower.hashAttackInterval, 1f);
    }
}
