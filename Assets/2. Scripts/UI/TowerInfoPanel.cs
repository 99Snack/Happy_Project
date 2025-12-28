using TMPro;
using UnityEngine;

public class TowerInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI attackIntervalText;
    [SerializeField] private TextMeshProUGUI sellText;
    [SerializeField] private TextMeshProUGUI gradeText;
    [SerializeField] private TextMeshProUGUI mainTypeText;
    [SerializeField] private TextMeshProUGUI subTypeText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void Setup(TowerBaseData tower)
    {
        var localStr = DataManager.Instance.LocalizationData;

        nameText.text = localStr[tower.Name].Ko;
        rangeText.text = $"{tower.Range} 칸";
        attackText.text = $"공격력 {tower.Attack}";
        attackIntervalText.text = $"공격 주기 {tower.AttackInterval}";
        sellText.text = $"판매 ({tower.price}골드)";
        gradeText.text = $"{tower.Grade} 성";

        string mainStr = tower.MainType switch
        {
            1 => "원거리",
            _ => "근접"
        };
        mainTypeText.text = mainStr;

        string subStr = tower.SubType switch
        {

            2 => "광역 공격형",
            3 => "광역 디버프형",
            _ => "단일 공격형",
        };
        subTypeText.text = subStr;

        descriptionText.text = localStr[tower.Desc].Ko;
    }
}
