using UnityEngine;
public static class AugmentFactory
{
    public static object CreateInstance(AugmentData augment)
    {
        //Debug.Log(augment.Index);
        return augment.Index switch
        {
            300004 => new TuberculosisAugment(augment),
            300005 => new OneHeartAugment(augment),
            300006 => new GoldDamageAugment(augment),
            300007 => new HarvestAugment(augment.Value_N),
            _ => null
        };
    }
}