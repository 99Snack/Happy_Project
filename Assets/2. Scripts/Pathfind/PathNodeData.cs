/*
    경로 노드의 데이터를 가지고 있는 클래스 
*/
using UnityEngine;

public class PathNodeData
{
    //▼ 해당 방향이 열려있는지 표시하는 상수 bool
    private const bool OPENED = true;
    private const bool CLOSED = false;
    //▼ 현재 타일 데이터 
    private TileData tileData;
    //▼ 해당 방향의 노드의 개폐 여부를 저장하는 배열 방향은 (동, 서, 남, 북, 현재 타일) 
    private bool[] isOpenDir;
    //▼ 막힌 노드까지의 거리를 나타내는 배열(동, 서, 남, 북, 현재 타일 0 고정)
    private int[] disToBlocks;
    //▼ 현재 노드의(x, y)좌표 
    private Vector2Int coor;

    /// <summary>
    /// PathNodeData 생성자 실제 좌표가 아닌 타일 상 좌표를 넣어서 생성
    /// </summary>
    /// <param name="X">x좌표</param>
    /// <param name="Y">y좌표</param>
    public PathNodeData(int X , int Y )
    {
        coor = new Vector2Int(X, Y);
        tileData = TileManager.Instance.GetTileData(coor.x, coor.y);
    }

    /// <summary>
    /// 해당 방향이 열려있는지 참/거짓 여부를 반환해주는 메서드 
    /// 참이면 열려있고 거짓이면 닫혀있다.
    /// </summary>
    /// <param name="direction">검사하고자하는 방향</param>
    /// <returns></returns>
    public bool GetOpenState(DIRECTION direction)
    {
       int index = ChangeDirectionToIndex(direction);
       
       return isOpenDir[index];
    }

    /// <summary>
    /// 방향을 해당하는 index값으로 변환해주는 메서드
    /// </summary>
    /// <param name="direction">변환하고자하는 방향</param>
    /// <returns></returns>
    public int ChangeDirectionToIndex(DIRECTION direction)
    {
        switch(direction)
        {
           case DIRECTION.East:
                return 0;
           case DIRECTION.West:
                return 1;
           case DIRECTION.South:
                return 2;
           case DIRECTION.North:
                return 3;
           case DIRECTION.None:
                return 4;
           default: 
                Debug.LogError("Unvaliable Direction");
                return -1;
        }
    }

    
}
