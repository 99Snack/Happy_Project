/*
    경로 생성의 전반적인 로직을 관리하는 클래스 
*/
using System;
using System.Collections.Generic;
using UnityEngine;

public class PathNodeManager : MonoBehaviour
{   
    //▼ 타일의 X와 Y 길이 상수
    private const int TILE_X_LENGTH = 14;
    private const int TILE_Y_LENGTH = 8;
    //▼ 해당 방향이 열려있는지 표시하는 상수 bool
    private const bool OPENED = true;
    private const bool CLOSED = false;

    private static PathNodeManager instance;
    public static PathNodeManager Instance => instance;
    private Vector2Int destinationPosition;
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

    /// <summary>
    /// 몬스터 경로 생성 후 경로 반환반기 
    /// </summary>
    /// <param name="spawnNum">스폰 번호</param>
    /// <param name="deadEndMoveLimit">막다른 길 감지 거리  </param>
    /// <param name="feedBack">피드백 좌표를 받을 배열</param>
    /// <returns>실제로 가야하는 경로</returns>
    public Vector2Int[] GetPathNode (int spawnNum, int deadEndMoveLimit , out Vector2Int[] feedBack)
    {
        bool isSucceedGenerte = GeneratePath();
        feedBack = Array.Empty<Vector2Int>();

        if(isSucceedGenerte)
        {
            return GetPathAndFeedBack(spawnNum, deadEndMoveLimit, out feedBack);
        }
        else
        {
            Debug.LogError("경로 없음");
            return Array.Empty<Vector2Int>();
        }
    }


    /// <summary>
    /// 경로 유효성 검사 후 경로 생성하는 메서드 
    /// </summary>
    public bool GeneratePath()
    {   
        pathes = new();
        destinationPosition = TileManager.Instance.allyBasePosition;
        pathGenerator = new PlayPathGenerator();
        GeneratePathNodeTiles();
        IsGenerated = false;
        SetBlockedStatus();
        CheckPathGenerate();
        
        if(IsGenerated)
        {
            GenerateRestPath();
        }
        else
        {
            //todo: 실패했을 떄 수행할 행동 
            Debug.LogError("경로 없음");
            
        }
        return IsGenerated;
    }


    /// <summary>
    /// 외부에서 호출되어 실제 경로와 
    /// 피드백을 출력할 노드들을 반환하는 메서드
    /// </summary>
    /// <param name="spawnNum">스폰 번호</param>
    /// <param name="deadEndMoveLimit">막다른 길 감지거리</param>
    /// <param name="feedBack">피드백을 출력할 노드 배열 </param>
    /// <returns>계산된 경로</returns>
    public Vector2Int[] GetPathAndFeedBack (int spawnNum, int deadEndMoveLimit , out Vector2Int[] feedBack)
    {
        
        Vector2Int[] basePath = GetBasePath(spawnNum);//기본 경로 
        Vector2Int[] calcPath ;//계산된 경로 
        Vector2Int targetVector = new Vector2Int(-1, -1); //찾아야하는 벡터 
        List<Vector2Int> feedBackList = new();//피드백 지점 
        Stack<Vector2Int> calculatedPath = new();//계산된 경로
        Stack<Vector2Int> calculateStack = new();//계산을 위한 스택
        
        bool isPathToDelete = false;//버려야 할 경로인지 여부

        for(int i = 0; i < basePath.Length; i++)
        {
            Vector2Int pathVector = basePath[i];

            if(i == 0 || i == basePath.Length - 1)
            {
                calculatedPath.Push(pathVector);
                continue;
            }

            if(isPathToDelete) //삭제 중
            {
                //▼ 타깃 벡터라면 삭제 모드 해제
                if(pathVector == targetVector) 
                {
                    isPathToDelete = false;
                    calculateStack.Clear();
                    continue;
                }
                else
                {
                    continue;
                } 
            }
            else if(i == 1 || i == basePath.Length - 2)
            {
                if(pathNodeTiles[pathVector.y,pathVector.x].GetOpenDirectionCount() + 1 > 2)
                {
                    calculateStack.Clear();
                    calculateStack.Push(pathVector);
                }
            }
            else if(pathNodeTiles[pathVector.y,pathVector.x].GetOpenDirectionCount() > 2)//분기점이라면 
            {
                calculateStack.Clear();
                calculateStack.Push(pathVector);
            }
            //▼ 분기점이 아닌데 피드백 계산 중 
            else if(calculateStack.Count > 0) 
            {
                //▼ 현재 벡터가 이미 지나왔던 길이고 분기점 부터 거리가 n보다 크다면 
                if(calculatedPath.Contains(pathVector) && calculateStack.Count > deadEndMoveLimit + 1 ) 
                {
                    isPathToDelete = true;

                    //▼ 반환점을 조건에 맞는 반환점을 제외한 경로 삭제  
                    while(calculateStack.Count > deadEndMoveLimit + 1)
                    {
                        calculateStack.Pop();
                        calculatedPath.Pop();   
                    }

                    targetVector = calculateStack.Peek();

                    //▼ 피드백 지점 리스트에 없다면 
                    if(!feedBackList.Contains(targetVector))
                    {
                        feedBackList.Add(targetVector);   
                    }
                }
                else if(calculatedPath.Contains(pathVector) && calculateStack.Count <= deadEndMoveLimit + 1 )
                {
                    calculateStack.Clear();
                }
                else
                {
                    calculateStack.Push(pathVector);
                }
            }
            //▼ 버려야할 경로가 아니라면 
            if(!isPathToDelete)
            {
                calculatedPath.Push(pathVector);
            }
            //▼ 이미 버려야할 경로를 전부 버렸다면
            else if(targetVector == pathVector && isPathToDelete)
            {
                isPathToDelete = false;
                calculateStack.Clear();
            }    
        }
        calcPath = new Vector2Int[calculatedPath.Count]; 

        for(int i = calculatedPath.Count - 1; i >= 0; i--)
        {
            calcPath[i] = calculatedPath.Pop();     
        }

        feedBack = feedBackList.ToArray();

        return calcPath;
    }



    /// <summary>
    /// 스폰 넘버를 통해 복사된 경로배열을 반환받는 메서드
    /// </summary>
    /// <param name="spawnNum">몬스터 스폰 번호 </param>
    /// <returns></returns>
    private Vector2Int[] GetBasePath(int spawnNum)
    {
        int pathNum = ChangeSpawnNumToRuleNum(spawnNum);
        Vector2Int [] returnPath = new Vector2Int[pathes[pathNum].Length];
        pathes[pathNum].CopyTo(returnPath, 0);
        return returnPath;
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

    /// <summary>
    /// 방향별 막힘 설정 
    /// </summary>
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
    /// <summary>
    /// 생성 여부 확인용 경로를 제외한 경로 생성 
    /// </summary>
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
   
    /// <summary>
    /// 스폰 번호 규칙 번호로 변환하는 메서드
    /// </summary>
    /// <param name="spawnNum"> 스폰 번호</param>
    /// <returns>규칙 번호</returns>
    private int ChangeSpawnNumToRuleNum(int spawnNum)
    {
        int ruleNumber = spawnNum % 4 - 1;
        if(ruleNumber == -1)
        {
            ruleNumber = 3;
        }

        return ruleNumber;
    }

   
}
