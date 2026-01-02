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

    Tower currentTower;
    
    public void Setup(Tower selectTower)
    {
        currentTower = selectTower;
        TowerBaseData tower = selectTower.Data;

        var localStr = DataManager.Instance.LocalizationData;

        nameText.text = localStr[tower.Name].Ko;
        rangeText.text = $"{tower.Range} 칸";
        attackText.text = $"공격력 {selectTower.atkPower.finalStat}";
        attackIntervalText.text = $"공격 주기 {tower.AttackInterval}";
        sellText.text = $"판매 ({tower.price}골드)";
        gradeText.text = $"{tower.Grade} 성";

        string mainStr = tower.MainType switch
        {
            2 => "원거리",
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
        HighlightRangeTile(true);
    }

    public void SellTower()
    {
        if (currentTower != null)
        {
            TowerManager.Instance.SellTower(currentTower);
            currentTower = null;

            gameObject.SetActive(false);
        }
    }
    
    public void HighlightRangeTile(bool isHighlight)
    {
        Vector2Int center = new Vector2Int(currentTower.MyTile.X,currentTower.MyTile.Y);

        int towerRange = currentTower.Data.Range;
        
        for(int x = -towerRange; x <=  towerRange; x++ )
        {
            for(int y = -towerRange; y <= towerRange; y++ )
            {
                Vector2Int temp = new Vector2Int(center.x + x, center.y + y);
                
                if(!TileManager.Instance.IsValidCoordinate(temp.x,temp.y)) continue;
                
                TileInteractor coortile = TileManager.Instance.map.tiles[(temp.x,temp.y)];
                
                if(coortile.Type == TileInfo.TYPE.Wait || coortile.Type == TileInfo.TYPE.EnemyBase || coortile.Type== TileInfo.TYPE.AllyBase) 
                    continue;
                
                if(isHighlight)
                    UIManager.Instance.TurnOnHighlightTile(coortile, false);
                else
                    UIManager.Instance.TurnOffHighlightTile(coortile);
            }
        }    
    }
}
