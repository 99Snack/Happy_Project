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

        // 실제 데미지 처리 로직만 실행
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