using DG.Tweening;
using TMPro;
using UnityEngine;

public class WaveResultPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clear;
    [SerializeField] private TextMeshProUGUI lose;

    /// <summary>
    /// 1 : 클리어, 0 : 패배
    /// </summary>
    /// <param name="result"></param>
    public void OnResultAnimation(int result)
    {
        TextMeshProUGUI targetText = result == 1 ? clear : lose;
        TextMeshProUGUI otherText = result == 0 ? clear : lose;

        targetText.gameObject.SetActive(true);
        otherText.gameObject.SetActive(false);

        targetText.alpha = 1f;

        targetText.DOFade(0f, 3f)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            UIManager.Instance.CloseWaveResultPanel();
        });
    }
}
