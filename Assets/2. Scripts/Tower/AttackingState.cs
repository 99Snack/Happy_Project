using UnityEngine;

public class AttackingState : ITowerState
{
    private Tower tower;

    public AttackingState(Tower tower)
    {
        this.tower = tower;
    }

    public void Enter()
    {
        //Debug.Log("attack ->");

        tower.attackCooldown = tower.Data.AttackInterval;

        tower.animator.SetFloat(tower.hashAttackInterval, tower.Data.AttackInterval);
        tower.animator.SetTrigger(tower.hashAttack);
        tower.animator.SetBool(tower.hashIsAttacking, true);
    }

    public void Update()
    {
        if (tower.currentTarget == null)
        {
            tower.ChangeState(new IdleState(tower));
            return;
        }

        if (tower.IsRotate)
        {
            Vector3 direction = (tower.Soldier.position - tower.currentTarget.transform.position);

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

        if (tower.IsTargetInRange()) return;

        tower.ChangeState(new AttackStopState(tower));
    }

    public void Exit()
    {
        tower.animator.SetBool(tower.hashIsAttacking, false);
    }
}
