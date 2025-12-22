using UnityEngine;

public abstract class RangeTower : Tower
{
    //protected override void Start()
    //{
    //    base.Start();
    //}

    //protected override void Update()
    //{
    //    base.Update();
    //}

    public override void Attack()
    {
        //기본 원거리 공격 방식 구현

        int enemyX = Mathf.FloorToInt(currentTarget.transform.position.x);
        int enemyY = Mathf.FloorToInt(currentTarget.transform.position.z);

        Vector2Int enemyTile = new Vector2Int(enemyX, enemyY);

        int dx = Mathf.Abs(Coord.x - enemyTile.x);
        int dy = Mathf.Abs(Coord.y - enemyTile.y);
        int distance = Mathf.Max(dx, dy);

        //Debug.Log($"{distance} : {tower.attackRange}");
        //사거리 벗어나면
        if (distance > attackRange)
        {
            currentTarget = null;
            ChangeState(AttackStopState);
            return;
        }
        // 사거리 안에 있으면
        else
        {
            if (attackCooldown <= 0f)
            {
                //shooter.Shoot(currentTarget, 0f, attackHitCount);
                shooter.Shoot(currentTarget, 1 , attackHitCount);
                attackCooldown = 1f / attackSpeed;
                animator.SetTrigger("AttackTrigger");
            }
        }
    }
}