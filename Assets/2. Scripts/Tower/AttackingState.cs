using System.Collections;
using UnityEngine;

public class AttackingState : ITowerState
{
    private Tower tower;
    private bool waitingForAnimation = false;

    public AttackingState(Tower tower)
    {
        this.tower = tower;
    }

    public void Enter()
    {
        tower.state = statetest.attack;
        tower.animator.SetBool(tower.hashIsAttacking, true);
        tower.animator.SetBool(tower.hashIsCooldown, false);
        waitingForAnimation = false;

        // 애니메이션 속도 조정
        AdjustAnimationSpeed();
    }

    public void Update()
    {
        // 타겟 유효성 검사
        if (tower.currentTarget == null || !tower.currentTarget.gameObject.activeInHierarchy)
        {
            tower.currentTarget = null;
            tower.ChangeState(tower.AttackStopState);
            return;
        }

        // 사거리 체크
        if (!tower.IsTargetInRange())
        {
            tower.ChangeState(tower.AttackStopState);
            return;
        }

        // 회전 처리
        if (tower.IsRotate)
        {
            Vector3 direction = tower.Soldier.position - tower.currentTarget.transform.position;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion lookTarget = Quaternion.LookRotation(direction);
                tower.Soldier.rotation = Quaternion.Slerp(
                    tower.Soldier.rotation,
                    lookTarget,
                    Time.deltaTime * 30f
                );
            }
        }

        // 쿨다운이 끝나고 애니메이션 대기 중이 아닐 때만 공격 시작
        if (tower.CanAttack() && !waitingForAnimation)
        {
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        waitingForAnimation = true;

        // 쿨다운 시작 (AttackInterval 보장)
        tower.ResetCooldown(tower.Data.AttackInterval);

        // 애니메이션 트리거
        tower.animator.SetTrigger(tower.hashAttack);

        // 애니메이션이 끝날 때까지 대기하는 코루틴 시작
        tower.StartCoroutine(WaitForAnimationEnd());
    }

    private IEnumerator WaitForAnimationEnd()
    {
        // 트리거 발동 후 애니메이션 전환 대기
        yield return null;

        AnimatorStateInfo stateInfo = tower.animator.GetCurrentAnimatorStateInfo(0);

        // Attack 태그가 있는 애니메이션이 끝날 때까지 대기
        while (stateInfo.IsTag("Attack") && stateInfo.normalizedTime < 0.95f)
        {
            yield return null;
            stateInfo = tower.animator.GetCurrentAnimatorStateInfo(0);
        }

        waitingForAnimation = false;
    }

    private void AdjustAnimationSpeed()
    {
        tower.StartCoroutine(SetAnimationSpeed());
    }

    private IEnumerator SetAnimationSpeed()
    {
        yield return null;

        AnimatorStateInfo stateInfo = tower.animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsTag("Attack"))
        {
            float animLength = stateInfo.length;

            if (animLength > 0)
            {
                float speed = animLength / tower.Data.AttackInterval;
                tower.animator.SetFloat(tower.hashAttackInterval, speed);
            }
        }
    }

    public void Exit()
    {
        tower.animator.SetBool(tower.hashIsAttacking, false);
        tower.animator.SetFloat(tower.hashAttackInterval, 1f);
        waitingForAnimation = false;
    }
}