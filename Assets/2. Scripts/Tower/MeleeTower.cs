
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

    public override void ApplyAugment(AugmentData augment)
    {
        base.ApplyAugment(augment);

        if (augment.Tag != 1) return;

        if(augment.Category == 1)
        { 
            UpdateStatus(augment);
        }
    }

    public override int CalcAttackOfficial()
    {
        return Data.Attack * Data.HitCount;
    }
}
