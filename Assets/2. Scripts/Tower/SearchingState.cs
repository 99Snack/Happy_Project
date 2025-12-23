using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SearchingState : ITowerState
{
    private Tower tower;
    WaitForSeconds searchDelay = new WaitForSeconds(0.1f);

    public SearchingState(Tower tower)
    {
        this.tower = tower;
    }

    public void Enter()
    {
        Debug.Log("search ->");

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

            yield return searchDelay;
        }
    }

    public void Update()
    {
        if (tower.currentTarget == null) return;

        tower.ChangeState(new AttackingState(tower));
    }

    public void Exit()
    {
        tower.animator.SetBool(tower.hashIsReady, false);

        tower.SearchingStopCoroutine();
    }
}
