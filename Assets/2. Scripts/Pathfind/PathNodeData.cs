/*
    경로 노드의 데이터를 가지고 변경하는 클래스 
*/
using UnityEngine;

public class PathNodeData
{
     //▼ 해당 방향이 열려있는지 표시하는 상수 bool
     private const bool OPENED = true;
     private const bool CLOSED = false;
     //▼ 현재 타일 데이터 
     private TileData tileData;
     //▼ 해당 방향의 노드의 개폐 여부를 저장하는 배열 방향은 (동, 서, 남, 북) 
     private bool[] isOpenDir;
     //▼ 막힌 노드까지의 거리를 나타내는 배열(동, 서, 남, 북)
     private int[] disToBlocks;
     //▼ 현재 노드의(x, y)좌표 
     private Vector2Int coordinate;
     //▼ 현재 노드가 실제로 막혔는지 여부 
     private bool isBlocked; 
     public bool IsBlocked => isBlocked;

     /// <summary>
     /// PathNodeData 생성자 실제 좌표가 아닌 타일 상 좌표를 넣어서 생성
     /// </summary>
     /// <param name="X">x좌표</param>
     /// <param name="Y">y좌표</param>
     public PathNodeData(int X, int Y)
     {
          coordinate = new Vector2Int(X, Y);
          tileData = TileManager.Instance.GetTileData(coordinate.x, coordinate.y);
          isOpenDir = new bool[4]{true,true,true,true};
          disToBlocks = new int[4];
          isBlocked = !(tileData.Type == TileData.TYPE.Road);
     }

     /// <summary>
     /// 현재 PathNodeData를 새로운 객체로 복사하는 메서드
     /// </summary>
     /// <returns></returns>
     public PathNodeData ClonePathNodeData()
     {
          PathNodeData temp = new PathNodeData(coordinate.x,coordinate.y);
          temp.CloneIsOpenDir(isOpenDir);
          temp.CloneDisToBlocks(disToBlocks);
          return temp;
     }

     /// <summary>
     /// IsOpenDir을 배열을 통해 복사받는 메서드
     /// </summary>
     /// <param name="toCopy">복사받을 배열</param>
     private void CloneIsOpenDir(bool[] toCopy)
     {
          toCopy.CopyTo(isOpenDir, 0);
         
     }

     /// <summary>
     /// DisToBlocks를 배열을 통해 복사받는 메서드
     /// </summary>
     /// <param name="toCopy">복사받을 배열</param>
     private void CloneDisToBlocks(int[] toCopy)
     {
          toCopy.CopyTo(disToBlocks, 0);
     }

     /// <summary>
     /// 모든 방향을 검사해 열려있는 방향이 있는지 확인하는 메서드 
     /// </summary>
     /// <returns></returns>
     public bool CheckEveryDirectionBlocked ()
     {
          foreach(bool status in isOpenDir)
          {
               if(status)
               {
                    return true;
               }
          }
          return false;
     }
     /// <summary>
     /// 열려있는 모든 방향을 담은 배열을 반환하는 메서드 
     /// </summary>
     /// <returns></returns>
     public DIRECTION[] GetAllOpenDirection()
     {
          DIRECTION[] openDir = new DIRECTION[]
          {
               DIRECTION.None, 
               DIRECTION.None, 
               DIRECTION.None, 
               DIRECTION.None
          };
          
          for(int i = 0; i < isOpenDir.Length; i++)
          {
               if(isOpenDir[i])
               {
                    openDir[i] = ChangeIndexToDirection(i);
               }
     }
          return openDir;
     }

     public int GetOpenDirectionCount()
     {
          int count = 0;
          
          for(int i = 0; i < isOpenDir.Length; i++)
          {
               if(isOpenDir[i] == OPENED)
                    count++;
          }
          return count;
     }



     /// <summary>
     /// 해당 방향이 열려있는지 참/거짓 여부를 반환해주는 메서드 
     /// 참이면 열려있다.
     /// </summary>
     /// <param name="direction">검사하고자하는 방향</param>
     /// <returns></returns>
     public bool GetOpenState(DIRECTION direction)
     {
          int index = ChangeDirectionToIndex(direction);

          return isOpenDir[index];
     }

     /// <summary>
     /// 막힌 곳까지 어느정도 거리가 남았는지를 반환해주는 메서드
     /// </summary>
     public int GetDistanceToBlocks(DIRECTION direction)
     {
          int index = ChangeDirectionToIndex(direction);

          return disToBlocks[index];
     }

     /// <summary>
     /// 웜하는 방향의 개폐여부를 변경하는 메서드
     /// </summary>
     /// <param name="direction">방향</param>
     /// <param name="status">개폐여부(참이 열림)</param>
     public void ChangeOpenStatus(DIRECTION direction, bool status)
     {
          int index = ChangeDirectionToIndex(direction);
          isOpenDir[index] = status;
     }

     /// <summary>
     /// 해당 방향으로 진행했을 때 막힌 블럭이 나오는 거리를 변경하는 메서드 
     /// </summary>
     /// <param name="direction"></param>
     /// <param name="distance"></param>
     public void ChangeDisToBlock(DIRECTION direction, int distance)
     {
          int index = ChangeDirectionToIndex(direction);
          disToBlocks[index] = distance;
     }

     /// <summary>
     /// 방향을 해당하는 index값으로 변환해주는 메서드
     /// DIRECTION.None은 잘못된 방향 
     /// </summary>
     /// <param name="direction">변환하고자하는 방향</param>
     /// <returns></returns>
     private int ChangeDirectionToIndex(DIRECTION direction)
     {
          switch (direction)
          {
               case DIRECTION.East:
                    return 0;
               case DIRECTION.West:
                    return 1;
               case DIRECTION.South:
                    return 2;
               case DIRECTION.North:
                    return 3;
               default:
                    Debug.LogError("Unvaliable Direction");
                    return -1;
          }
      }
     /// <summary>
     /// index값에 해당하는 방향을 반환해주는 메서드 
     /// </summary>
     /// <param name="index">변환하고자 하는 인덱스</param>
     /// <returns></returns>
     public DIRECTION ChangeIndexToDirection(int index)
     {
          switch(index)
          {    
               case 0:
                    return DIRECTION.East;
               case 1:
                    return DIRECTION.West;
               case 2:  
                    return DIRECTION.South;
               case 3:
                    return DIRECTION.North;
               default:
                    Debug.LogError("Unvaliable Index");
                    return DIRECTION.None; 
          }
     }
     /// <summary>
     /// 현재 노드를 막는 메서드
     /// </summary>
     public void BlockPathNode()
     {
          isBlocked = true;
     }

}
