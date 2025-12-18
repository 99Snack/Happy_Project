using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileInteractor : MonoBehaviour, IPointerClickHandler
{
    public int X;
    public int Y;

    public bool isAlreadyTower = false;
    public TileData.TYPE Type = TileData.TYPE.None;

    private void Start()
    {
        X = (int)transform.position.x;
        Y = (int)transform.position.z;
    }

    public void Setup(int x,int y, TileData.TYPE type){
        this.X = x;
        this.Y = y;
        Type = type;
    }


    //타일 전환 기능
    public void OnPointerClick(PointerEventData eventData)
    {
        TileData data = TileManager.Instance.GetTileData(X,Y);

        if (!data.IsTransition || isAlreadyTower) return;


        if (data.Type == TileData.TYPE.Wall)
        {
            Type = TileData.TYPE.Road;
        }
        else
        {
            Type = TileData.TYPE.Wall;
        }

        TileManager.Instance.ChangeType(X, Y, Type);

        bool isWall = data.Type == TileData.TYPE.Wall ? true : false;

        transform.GetChild(0).gameObject.SetActive(isWall);
        transform.GetChild(1).gameObject.SetActive(!isWall);
    }

}
