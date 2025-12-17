using UnityEngine;

public class TileInteractor : MonoBehaviour
{
    public int x;
    public int y;

    public TileData.TYPE type;

    public void Setup(int x, int y, TileData.TYPE type){
        this.x = x;
        this.y = y;
        this.type = type;  
    }
}
