using UnityEngine;

public class AnimationEventProxy : MonoBehaviour
{
    Tower tower;

    private void Awake()
    {
        tower = GetComponentInParent<Tower>();
    }

    // 애니메이션 중간에 호출되는 실제 공격 실행 지점
    public void OnAttack()
    {
        if (tower == null || tower.currentTarget == null) return;

        ClipName clip = tower switch
        {
            MineThrowerTower => ClipName.Cannon_sound,
            MageTower => ClipName.Magic_sound,
            IceMageTower => ClipName.Ice_sound,
            SpearTower => ClipName.Spear_sound,
            _ => ClipName.Knight_sound,
        };

        SoundManager.Instance.PlaySFX(clip);

        tower.ExecuteDamage();
    }

    // 공격 준비 이펙트 (마법사 등)
    public void AttackReady()
    {
        if (tower is MageTower mage)
        {
            mage.HitEffect();
        }
    }
}