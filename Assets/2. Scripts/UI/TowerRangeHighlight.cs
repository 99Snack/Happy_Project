
using System.Collections.Generic;
using UnityEngine;

public class TowerRangeHighlight
{
    Tower currentTower;
    int range;
    static List<Vector2Int> highlightedtiles;

    public void Setup(Tower selectTower)
    {
        currentTower = selectTower;
        HighlightRangeTile();
    }

    public void TurnOff()
    {
        if (highlightedtiles != null && highlightedtiles.Count > 0)
        {
            foreach (var tile in highlightedtiles)
            {
                TileInteractor coortile = TileManager.Instance.map.tiles[(tile.x, tile.y)];
                UIManager.Instance.TurnOffHighlightTile(coortile);
            }
            highlightedtiles.Clear();
        }

    }

    public void HighlightRangeTile()
    {
        TurnOff();

        highlightedtiles = new List<Vector2Int>();

        Vector2Int center = new Vector2Int(currentTower.MyTile.X, currentTower.MyTile.Y);

        int towerRange = currentTower.Data.Range;

        for (int x = -towerRange; x <= towerRange; x++)
        {
            for (int y = -towerRange; y <= towerRange; y++)
            {
                Vector2Int temp = new Vector2Int(center.x + x, center.y + y);

                if (!TileManager.Instance.IsValidCoordinate(temp.x, temp.y)) continue;

                TileInteractor coortile = TileManager.Instance.map.tiles[(temp.x, temp.y)];

                if (temp.x == 0 && temp.y == 0 && coortile.Type == TileInfo.TYPE.Wait)
                {
                    return;
                }

                if (coortile.Type == TileInfo.TYPE.Wait || coortile.Type == TileInfo.TYPE.EnemyBase || coortile.Type == TileInfo.TYPE.AllyBase)
                    continue;

                UIManager.Instance.TurnOnHighlightTile(coortile, false);

                highlightedtiles.Add(temp);
            }
        }
    }
}
