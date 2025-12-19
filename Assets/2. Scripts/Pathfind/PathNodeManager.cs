/*
    경로 생성의 전반적인 로직을 관리하는 클래스 
*/

using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PathNodeManager : MonoBehaviour
{   
    //▼ 타일의 X와 Y 길이 상수
    private const int TILE_X_LENGTH = 14;
    private const int TILE_Y_LENGTH = 8;
    //▼ 해당 방향이 열려있는지 표시하는 상수 bool
    private const bool OPENED = true;
    private const bool CLOSED = false;
    private Vector2Int startPosition;
    private Vector2Int destinationPosition;

    private static PathNodeManager instance;
    private static PathNodeManager Instance => instance;
    //▼ 생성된 경로를 저장하는 딕셔너리
    private Dictionary<int, Vector2Int[]> pathes;
    //▼ 실제 PathNodeData를 담고 잇는 배열 (y, x)
    PathNodeData[ , ] pathNodeTiles;
    //▼경로를 생성용 PlayPathGenerator
    PlayPathGenerator pathGenerator;
    //▼ 경로가 생성되었는지 여부 
    bool IsGenerated;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }

    public void GeneratePath()
    {   pathes = new();
        destinationPosition = TileManager.Instance.allyBasePosition;
        pathGenerator = new PlayPathGenerator();
        GeneratePathNodeTiles();
        IsGenerated = false;
        SetBlockedStatus();
        CheckPathGenerate();
        
        if(IsGenerated)
        {
            Debug.Log("경로 생성 성공");

            // SetDistanceToBlock();
            // GenerateRestPath();
        }
        else
        {
            //todo: 실패했을 떄 수행한 행동 
            Debug.LogError("경로 없음");
        }
    }

    /// <summary>
    /// PathNodeTIles를 생성하는 메서드 
    /// </summary>
    private void GeneratePathNodeTiles()
    {   
        pathNodeTiles = new PathNodeData[TILE_Y_LENGTH, TILE_X_LENGTH];

        for (int i = 0; i < TILE_X_LENGTH; i++)
        {
            for(int j = 0; j < TILE_Y_LENGTH; j++ )
            {
                pathNodeTiles[j, i] = new PathNodeData(i, j); 
            }
        }
    }

    private void SetBlockedStatus()
    {
        pathNodeTiles[destinationPosition.y + 1, destinationPosition.x].BlockPathNode();
        pathNodeTiles[destinationPosition.y - 1, destinationPosition.x].BlockPathNode();

        for (int i = 0; i < TILE_X_LENGTH; i++)
        {
            for(int j = 0; j < TILE_Y_LENGTH; j++ )
            {
                //pathNodeTiles[y,x] [j,i]

                if(j + 1 >= TILE_Y_LENGTH || pathNodeTiles[j + 1, i].IsBlocked )//최상단 또는 상단이 막혔을 때 
                {
                    pathNodeTiles[j, i].ChangeOpenStatus(DIRECTION.North, CLOSED);
                }
                if(j - 1 < 0 || pathNodeTiles[j - 1, i].IsBlocked )//최하단 또는 하단이 막혔을 떄
                {
                    pathNodeTiles[j, i].ChangeOpenStatus(DIRECTION.South, CLOSED);
                }
                if(i + 1 >= TILE_X_LENGTH || pathNodeTiles[j, i + 1].IsBlocked) //최우측 또는 우측이 막혔을 떄 
                {
                    pathNodeTiles[j,i].ChangeOpenStatus(DIRECTION.East, CLOSED);
                }
                if(i - 1 < 0 || pathNodeTiles[j , i - 1].IsBlocked)//최좌측 또는 좌측이 막혔을 때 
                {
                    pathNodeTiles[j,i].ChangeOpenStatus(DIRECTION.West, CLOSED);
                }
            }
        }
    }
    /// <summary>
    /// 경로가 생성될 수 있으면 생성하고 0번쨰 경로에 넣는다. 
    /// </summary>
    /// <param name="ruleNum"></param>
    private void CheckPathGenerate()
    {
        Vector2Int[] outTemp;
        
        IsGenerated = pathGenerator.GetPath(pathNodeTiles, 0, out outTemp);
        
        pathes[0] = new Vector2Int[outTemp.Length];
        
        if(IsGenerated)
        {
            outTemp.CopyTo(pathes[0],0); 
        }
       
    }
    private void GenerateRestPath()
    {
        for(int i = 1; i < 4; i++)
        {
            Vector2Int[] outTemp;
            pathGenerator.GetPath(pathNodeTiles, i, out outTemp);
            pathes[i] = new Vector2Int[outTemp.Length];
            outTemp.CopyTo(pathes[i],0);
        }
    }

    private void SetDistanceToBlock()
    {
        Debug.Log("Bound 값 생성");
    }

    /// <summary>
    /// 스폰 번호 규칙 번호로 변환하는 메서드
    /// </summary>
    /// <param name="spawnNum">변환하고자하는 스폰 번호</param>
    /// <returns></returns>
    private int ChangeSpawnNumToRuleNum(int spawnNum)
    {
        int ruleNumber = spawnNum % 4;

        return ruleNumber;
    }
    
}
