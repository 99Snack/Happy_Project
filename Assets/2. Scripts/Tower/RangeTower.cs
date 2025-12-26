
using static UnityEngine.Rendering.HableCurve;

public class RangeTower : Tower
{
    //protected override void Start()
    //{
    //    base.Start();
    //}

    //protected override void Update()
    //{
    //    base.Update();
    //}

    public override void ApplyAugment(AugmentData augment)
    {
        base.ApplyAugment(augment);

        if (augment.Tag != 2) return;

        if (augment.Category != 3)
        {
            UpdateStatus(augment);
        }
    }

    public override int CalcAttackOfficial()
    {
        return Data.Attack * Data.HitCount;
    }
}