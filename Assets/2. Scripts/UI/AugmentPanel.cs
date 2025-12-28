using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AugmentPanel : MonoBehaviour
{
    [SerializeField] private GameObject AugmentPopup;
    public AugmentItem[] items;
    public TextMeshProUGUI skipText;
    private bool isVisible = true;

    private void Start()
    {
        isVisible = true;
    }

    public void Setup(List<AugmentData> augments)
    {
        for (int i = 0; i < augments.Count; i++)
        {
            items[i].Setup(augments[i]);
        }
    }

    public void VisiblePopup()
    {
        isVisible = !isVisible;
        AugmentPopup.SetActive(isVisible);
    }

    public void SkipButton(){
        //-Tier 및 스테이지 비례 계산된 골드량을 계산 후 표기
        //골드 기본 수치 x(1 + { 스테이지 - 1}) x 골드 성장 계수) = 해당 스테이지에서 선택 포기로 얻는 골드량
        //int stage = GameManager.Instance.StageInfo.Index - 10000;
        //int g_value = GameManager.Instance.StageInfo
        //int skipGold = 100 * (1 + (stage - 1) * g_value);

        //skipText.text = $"포기하기 +({skipGold})골드";

        //GameManager.Instance.Gold += skipGold;
    }
}
