public static class AugmentFactory
{
    //public static object CreateInstance(int id)
    //{
    //    return id switch
    //    {
    //        101 => new ChainAttack(),
    //        102 => new LonelyBonus(),
    //        // 새로운 증강은 여기에 ID만 추가하면 끝!
    //        _ => null
    //    };

    //}

    public static object CreateInstance(AugmentData augment)
    {
        return augment.Index switch
        {
            300004 => new TuberculosisAugment(),
            300005 => new OneHeartAugment(),
            300006 => new GoldDamageAugment(),
            300007 => new HarvestAugment(augment.Value_N),
            _ => null
        };
    }
}