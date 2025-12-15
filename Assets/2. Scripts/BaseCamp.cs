using UnityEngine;

public class BaseCamp : MonoBehaviour
{
    public TileData TileData { get; private set; }

    public void InitializeTileData(int x, int y)
    {
        TileData = new TileData(x, y, TileData.TYPE.Base);
    }
}
