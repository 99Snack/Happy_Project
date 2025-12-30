using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AugmentPanel : MonoBehaviour
{
    [SerializeField] private GameObject AugmentPopup;
    public AugmentItem[] items;
    public TextMeshProUGUI skipText;
    private bool isVisible = true;
    private int tier = 1;
    private int skipGold;

    private void Start()
    {
        isVisible = true;
    }

    public void Setup(List<AugmentData> augments)
    {
        tier = augments[0].Tier;
        for (int i = 0; i < augments.Count; i++)
        {
            items[i].Setup(augments[i]);
        }
        UpdateSkipGoldText();
    }

    public void VisiblePopup()
    {
        isVisible = !isVisible;
        AugmentPopup.SetActive(isVisible);
    }

    private void UpdateSkipGoldText()
    {
        //-Tier 및 스테이지 비례 계산된 골드량을 계산 후 표기
        //골드 기본 수치 x(1 + { 스테이지 - 1}) x 골드 성장 계수) = 해당 스테이지에서 선택 포기로 얻는 골드량
        int stage = GameManager.Instance.StageInfo.Index - 10000;
        int tierGold = tier switch
        {
            1 => 100,
            2 => 200,
            3 => 400,
            _ => 100,
        };
        skipGold = tierGold * (1 + (stage - 1));
        skipText.text = $"포기하기 +({skipGold})골드";
    }

    public void SkipButton()
    {
        GameManager.Instance.Gold += skipGold;

        gameObject.SetActive(false);
    }
}
