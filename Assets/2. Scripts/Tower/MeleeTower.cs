using UnityEngine;

public class MeleeTower : Tower
{
    protected override void Start()
    {
        base.Start();
        Debug.Log("Melee Start");
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Attack()
    {
        Debug.Log("근접 공격");
    }
}
