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
        //Debug.Log("search ->");

        searchDelay = new WaitForSeconds(0.1f);

        tower.animator.SetBool(tower.hashIsReady, true);

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

            //Debug.Log(tower.currentTarget);

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
