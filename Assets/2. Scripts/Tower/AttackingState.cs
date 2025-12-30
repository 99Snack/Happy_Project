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

        // [수정] 애니메이션 배속 설정
        if (tower.Data.AttackInterval > 0)
        {
            // [수정]배속 = 애니메이션원본길이 / 원하는공격간격
            float speedMultiplier = tower.AttackClipLength / tower.Data.AttackInterval;
            tower.animator.SetFloat(tower.hashAttackInterval, speedMultiplier);
        }
        

        tower.animator.SetBool(tower.hashIsAttacking, true);
        tower.animator.SetBool(tower.hashIsCooldown, false);

        waitingForAnimation = false;

        // [수정]진입하자마자 공격
        if (tower.CanAttack())
        {
            ExecuteAttack();
        }
    }
    
    private void ApplySpeedImmediate()
    {
        // 애니메이션 길이에 맞춰 AttackInterval 파라미터 즉시 설정
        // 기본 배속 계산 로직 (기본 1배속 설정 후 애니메이터가 배속 적용)
        //float animLength = 1.0f; // 기본값
        //if (tower.animator.runtimeAnimatorController != null)
        //{
        //    float speed = 1.0f / tower.Data.AttackInterval;
        //    tower.animator.SetFloat(tower.hashAttackInterval, speed);
        //}

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

    public void Update()
    {
        if (tower.currentTarget == null || !tower.currentTarget.gameObject.activeInHierarchy)
        {
            tower.currentTarget = null;
            tower.ChangeState(tower.AttackStopState);
            return;
        }

        if (!tower.IsTargetInRange())
        {
            tower.ChangeState(tower.AttackStopState);
            return;
        }

        if (tower.IsRotate)
        {
            // 타겟 방향 바라보기 (부드러운 회전)
            Vector3 dir = tower.currentTarget.transform.position - tower.transform.position;
            dir.y = 0;
            if (dir != Vector3.zero)
                tower.Soldier.rotation = Quaternion.Slerp(tower.Soldier.rotation, Quaternion.LookRotation(-dir), Time.deltaTime * 15f);
        }

        // 공격 가능하면 즉시 공격
        if (tower.CanAttack() && !waitingForAnimation)
        {
            tower.Attack();
            tower.attackCooldown = tower.Data.AttackInterval;
            tower.StartCoroutine(WaitForAnimationEnd());
        }
    }

    // [수정]
    private void ExecuteAttack()
    {
        tower.Attack();
        tower.attackCooldown = tower.Data.AttackInterval;

        // 애니메이션을 0프레임부터 강제 재생하여 딜레이 체감 제거
        tower.animator.Play("Attack", 0, 0f);

        if (tower.gameObject.activeInHierarchy)
        {
            tower.StartCoroutine(WaitForAnimationEnd());
        }
    }

    private IEnumerator WaitForAnimationEnd()
    {
        waitingForAnimation = true;
        yield return new WaitForSeconds(tower.Data.AttackInterval * 0.9f); // 공격 간격의 90% 지점에서 해제
        waitingForAnimation = false;
    }

    public void Exit() {
        tower.animator.SetBool(tower.hashIsAttacking, false);
    }
}