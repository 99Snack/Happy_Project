using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyBaseCampPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI allyBaseCampHpText;
    [SerializeField] private Slider allyBaseCampHpSlider;

    public void UpdateAllyBaseCampHp()
    {
        int current = BaseCamp.Instance.CurrentHp;
        int maxHp = BaseCamp.Instance.basecampHp;

        allyBaseCampHpText.text = $"{current}/{maxHp}";

        float value = (float)current / maxHp;
        allyBaseCampHpSlider.DOValue(value, 0.25f).SetEase(Ease.Linear);
    }

}
