using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AugmentPanel : MonoBehaviour
{
    [SerializeField] private GameObject AugmentPopup;

    [SerializeField] private GameObject silverAugment;
    [SerializeField] private GameObject goldAugment;
    [SerializeField] private GameObject prismAugment;

    [SerializeField] private Transform augmentItemParent;

    List<Vector3> pos = new List<Vector3>
    {
    new Vector3(-400, 65, 0),
    new Vector3(0, 65, 0),
    new Vector3(400, 65, 0)
    };

    List<GameObject> augmentItems = new List<GameObject>();

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
        ListClear();

        tier = augments[0].Tier;
        GameObject prefab = tier switch
        {
            1 => silverAugment,
            2 => goldAugment,
            3 => prismAugment,
            _ => silverAugment
        };

        for (int i = 0; i < augments.Count; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.transform.SetParent(augmentItemParent);
            obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos[i].x, pos[i].y);
            augmentItems.Add(obj);
            AugmentItem item = obj.GetComponent<AugmentItem>();
            item.Setup(augments[i]);
        }
        UpdateSkipGoldText();
    }

    private void ListClear()
    {

        foreach (var item in augmentItems)
        {
            Destroy(item);
        }

        augmentItems.Clear();
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
