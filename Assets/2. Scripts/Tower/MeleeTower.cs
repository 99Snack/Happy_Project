
using System.Data;

public class MeleeTower : Tower
{
    //protected override void Start()
    //{
    //    base.Start();
    //    Debug.Log("Melee Start");
    //}

    //protected override void Update()
    //{
    //    base.Update();
    //}
    public override void Attack(MonsterMove monster)
    {
        if (monster == null) return; 

        int attackPower = CalcAttackPower();
     
        monster.OnHit(attackPower);

        if (attackCooldown <= 0f)
        {
            attackCooldown = Data.AttackInterval;
            animator.SetTrigger(hashAttack);
        }
    }

}
