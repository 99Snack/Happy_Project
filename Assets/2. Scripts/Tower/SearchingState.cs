using System.Collections;
using UnityEngine;

public class SearchingState : ITowerState
{
    private Tower tower;
    private WaitForSeconds searchDelay;

    public SearchingState(Tower tower)
    {
        this.tower = tower;
        searchDelay = new WaitForSeconds(0.05f); // 탐색 주기를 더 빠르게 (0.1f -> 0.05f)
    }

    public void Enter()
    {
        tower.state = statetest.search;
        tower.animator.SetBool(tower.hashIsReady, true);
        tower.animator.SetBool(tower.hashIsAttacking, false);
        tower.animator.SetBool(tower.hashIsCooldown, false);

        // 설치 즉시 공격할 수 있도록 쿨다운 초기화
        tower.attackCooldown = 0f;

        // 들어가자마자 한 번 즉시 탐색
        SearchImmediate();

        tower.SearchingCoroutine(FindNearestEnemyInRange());
    }

    private void SearchImmediate()
    {
        tower.currentTarget = TowerTargetDetector.Instance.FindNearestEnemyInRange(
            tower.Coord,
            tower.Data.Range,
            TileManager.Instance.allyBasePosition
        );

        if (tower.currentTarget != null)
        {
            tower.ChangeState(tower.AttackingState);
        }
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

            if (tower.currentTarget != null)
            {
                // 타겟 발견 시 Update까지 기다리지 않고 즉시 상태 전환 시도
                tower.ChangeState(tower.AttackingState);
                yield break;
            }

            yield return searchDelay;
        }
    }

    public void Update()
    {
        // 코루틴 외에도 Update에서 안전장치로 체크
        if (tower.currentTarget != null)
        {
            tower.ChangeState(tower.AttackingState);
        }
    }

    public void Exit()
    {
        tower.SearchingStopCoroutine();
    }
}