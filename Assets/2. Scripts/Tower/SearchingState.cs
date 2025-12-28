using System.Collections;
using UnityEngine;

public class SearchingState : ITowerState
{
    private Tower tower;
    WaitForSeconds searchDelay;

    public SearchingState(Tower tower)
    {
        this.tower = tower;
    }

    public void Enter()
    {
        tower.state = statetest.search;

        searchDelay = new WaitForSeconds(0.1f);

        // Search 상태 활성화
        tower.animator.SetBool(tower.hashIsReady, true);
        tower.animator.SetBool(tower.hashIsAttacking, false);
        tower.animator.SetBool(tower.hashIsCooldown, false);

        //새 타겟 발견 시 즉시 공격
        tower.attackCooldown = 0f;

        tower.SearchingCoroutine(FindNearestEnemyInRange());
    }

    IEnumerator FindNearestEnemyInRange()
    {
        while (true)
        {
            tower.currentTarget = TowerTargetDetector.Instance.FindNearestEnemyInRange(
               tower.Coord,
               tower.Data.Range,
               TileManager.Instance.allyBasePosition
            );

            yield return searchDelay;
        }
    }

    public void Update()
    {
        if (tower.currentTarget == null) return;

        tower.ChangeState(tower.AttackingState);
    }

    public void Exit()
    {
        tower.animator.SetBool(tower.hashIsReady, false);
        tower.SearchingStopCoroutine();
    }
}