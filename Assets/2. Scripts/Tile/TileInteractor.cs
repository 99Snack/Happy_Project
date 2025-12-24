using UnityEngine;
using UnityEngine.EventSystems;

public class TileInteractor : MonoBehaviour, IPointerClickHandler
{
    public int X;
    public int Y;

    public bool isAlreadyTower = false;
    public TileInfo.TYPE Type = TileInfo.TYPE.None;

    private void Start()
    {
        X = (int)transform.position.x;
        Y = (int)transform.position.z;
    }

    public void Setup(int x,int y, TileInfo.TYPE type){
        this.X = x;
        this.Y = y;
        Type = type;
    }

    
    //타일 전환 기능
    public void OnPointerClick(PointerEventData eventData)
    {
        //타일 건설 불가 상태면 무시 
        //if (SpawnManager.Instance != null && !SpawnManager.Instance.CanBuild) return;
    
        //유효한 좌표가 아니면
        if (!TileManager.Instance.IsValidCoordinate(X, Y)) return;

        TileInfo data = TileManager.Instance.GetTileInfo(X,Y);
        
        if (!data.IsTransition || isAlreadyTower) return;
        
        //선택된 좌표 하이라이트
        transform.GetChild(3).gameObject.SetActive(true);
        
        UIManager.Instance.OpenTileTransitionPanel(this);
    }

    public void ChangeTileType()
    {
        TileInfo data = TileManager.Instance.GetTileInfo(X,Y);

        if (data.Type == TileInfo.TYPE.Wall)
        {
            Type = TileInfo.TYPE.Road;
        }
        else
        {
            Type = TileInfo.TYPE.Wall;
        }

        TileManager.Instance.ChangeType(X, Y, Type);

        bool isWall = data.Type == TileInfo.TYPE.Wall ? true : false;

        transform.GetChild(0).gameObject.SetActive(isWall);
        transform.GetChild(1).gameObject.SetActive(!isWall);
    }
    
}
