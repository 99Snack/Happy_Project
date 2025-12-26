using UnityEngine;

public class AnimationEventProxy : MonoBehaviour
{
    Tower tower;

    private void Awake()
    {
        tower = GetComponentInParent<Tower>();
    }

    public void OnAttack(){
        if (tower.currentTarget == null) return;

        tower.Attack();
    }

    public void AttackReady(){
        if(tower is MageTower){
            (tower as MageTower)?.HitEffect();
        }
    }
}
