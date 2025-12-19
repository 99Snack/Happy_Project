using NUnit.Framework.Interfaces;
using System.Collections;
using UnityEngine;

public class AttackStopState : ITowerState
{
    private Tower tower;
    float timer = 0;
    float duration = 0.5f;

    public AttackStopState(Tower tower)
    {
        this.tower = tower;
    }

    public void Enter()
    {
        Debug.Log("[타워] AttackStop 상태 진입");
        tower.animator.SetBool("isAttackStop", true);
        // Animator Event에서 Tower.OnAttackStopEnd() 호출
    }

    public void Update()
    {
        if (timer < duration)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            tower.ChangeState(new IdleState(tower));
        }
    }

    public void Exit()
    {
        tower.animator.SetBool("isAttackStop", false);
    }
}
