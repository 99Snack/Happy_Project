/*
    실제 경로를 생성하는 클래스 
*/
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayPathGenerator 
{
    //▼ 타일의 X와 Y 길이 상수
    private const int TILE_X_LENGTH = 14;
    private const int TILE_Y_LENGTH = 8;
    //▼ 해당 방향이 열려있는지 표시하는 상수 bool
    private const bool OPENED = true;
    private const bool CLOSED = false;

    //▼ 적군 베이스 캠프 위치
    private Vector2Int startPosition; 
    //▼ 아군 베이스 캠프 위치
    private Vector2Int destinationPosition;
    //▼ 경로 
    private Vector2Int[] path;
    //▼ 규칙 번호 
    private int ruleNumber;
    //▼ 방향을 선택하는 우선순위를 담은 배열
    private DIRECTION[] currentDirPriority; 
    //▼ 규칙 번호와 우선순위 배열을 연결해주는 딕셔너리
    private Dictionary<int, DIRECTION[]>dirPriorities;
    //▼ 경로 노드 정보 배열
    private PathNodeData[ , ] pathNodes;
    //▼ 반대 방향 계산을 위한 DirectionCalculator
    private DirectionCalculator dirCalculator;
    //▼경로 생성 성공 여부
    private bool isSuccess;
    
    public PlayPathGenerator()
    {
        Init();
    }
    
    private void Init()
    {
        startPosition = TileManager.Instance.enemyBasePosition;
        destinationPosition = TileManager.Instance.allyBasePosition;
        isSuccess = false;
        dirCalculator = new();
        InitDirPriorities();
    }

    /// <summary>
    /// 규칙별 우선 순위 추가 메서드 
    /// </summary>
    private void InitDirPriorities()
    {
        dirPriorities = new();
        dirPriorities[0] = new DIRECTION[]{DIRECTION.North,DIRECTION.East,DIRECTION.West,DIRECTION.South};
        dirPriorities[1] = new DIRECTION[]{DIRECTION.East,DIRECTION.West,DIRECTION.South,DIRECTION.North};
        dirPriorities[2] = new DIRECTION[]{DIRECTION.West,DIRECTION.South,DIRECTION.North,DIRECTION.East};
        dirPriorities[3] = new DIRECTION[]{DIRECTION.South,DIRECTION.North,DIRECTION.East,DIRECTION.West};
    }
    
    /// <summary>
    /// pathNodeDatas를 복사하는 메서드
    /// </summary>
    /// <param name="pathNodeDatas"></param>
    public void ClonePathNodeDatas(PathNodeData[ , ] pathNodeDatas)
    {
        pathNodes = new PathNodeData[TILE_Y_LENGTH , TILE_X_LENGTH]; 
        
        for(int i = 0; i < TILE_X_LENGTH; i++)
        {
            for (int j = 0; j < TILE_Y_LENGTH; j++)
            {
                pathNodes[j, i] = pathNodeDatas[j, i].ClonePathNodeData(); 
            }
        }
    }
    
    /// <summary>
    /// 경로 성공여부를 반환해주고 성공했을 시 path 또한 반환하는 메서드 
    /// </summary>
    /// <param name="pathNodeDatas">경로 노드 정보들</param>
    /// <param name="ruleNum">규칙 번호</param>
    /// <param name="path">경로를 받을 배열</param>
    /// <returns></returns>
    public bool GetPath(PathNodeData[ , ] pathNodeDatas, int ruleNum, out Vector2Int[] getPath)
    {

        ruleNumber = ruleNum;
        currentDirPriority = dirPriorities[ruleNumber];
        
        ClonePathNodeDatas(pathNodeDatas);
        
        GeneratePath();
        
        if(isSuccess)
        { 
            getPath = new Vector2Int[path.Length];
            path.CopyTo(getPath, 0);
           
        }
        else
        {
            getPath = new Vector2Int[0];
        }

        return isSuccess;        
    }

    /// <summary>
    /// 4 방향이 이미 지나 온 경로인지 체크하는 메서드
    /// </summary>
    /// <param name="passed">지나온 경로가 저장되어있는 스택</param>
    /// <param name="currentPos">현재 좌표</param>
    private DIRECTION[] Check4Direction(Stack<Vector2Int> passed, Vector2Int currentPos)
    {   
        DIRECTION[] closeDir = new DIRECTION[]{DIRECTION.None,DIRECTION.None,DIRECTION.None,DIRECTION.None};

        if(passed.Contains(new Vector2Int(currentPos.x + 1,currentPos.y)))
        {
            pathNodes[currentPos.y,currentPos.x].ChangeOpenStatus(DIRECTION.East,CLOSED);
            closeDir[0] = DIRECTION.East;  
        }
        if(passed.Contains(new Vector2Int(currentPos.x - 1,currentPos.y)))
        {
            pathNodes[currentPos.y,currentPos.x].ChangeOpenStatus(DIRECTION.West,CLOSED);
            closeDir[1] = DIRECTION.West;
        }
        if(passed.Contains(new Vector2Int(currentPos.x,currentPos.y - 1)))
        {
            pathNodes[currentPos.y,currentPos.x].ChangeOpenStatus(DIRECTION.South,CLOSED);
            closeDir[2] = DIRECTION.South;
        }
        if(passed.Contains(new Vector2Int(currentPos.x,currentPos.y + 1)))
        {
            pathNodes[currentPos.y,currentPos.x].ChangeOpenStatus(DIRECTION.North,CLOSED);
            closeDir[3] = DIRECTION.North;
        }
        return closeDir;
    }
    
    /// <summary>
    /// 실제로 길을 생성하는 메서드
    /// </summary>
    private void GeneratePath()
    {
       
        int calX = startPosition.x; //계산을 위한 x좌표
        int calY = startPosition.y; //계산을 위한 y좌표 
       
        Stack<Vector2Int> passed = new(); //지나온 경로를 저장하는 스택
        
        passed.Push(new Vector2Int(calX,calY));//시작 좌표 추가 

        DIRECTION selected = DIRECTION.None; //선택된 방향
        DIRECTION backDir = DIRECTION.None; //되돌아가는 방향 
        DIRECTION[] tempClosedDirections= new DIRECTION[]{DIRECTION.None,DIRECTION.None,DIRECTION.None,DIRECTION.None}; //임시로 닫아놓은 방향 
        DIRECTION[] openDirection; //열려있는 방향 

        bool isBackTraking = false; //백 트래킹 여부 

        while(true)
        {   
            // ▼ 지나온 경로가 있다면 주변에 있는지 체크 
            if(passed.Count > 0)
            {
              DIRECTION[] temp = Check4Direction(passed,new Vector2Int(calX,calY));
              temp.CopyTo(tempClosedDirections,0);
            }
            //▼ 선택된 방향이 있다면 되돌아가는 방향을 저장 
            if(selected != DIRECTION.None)
            {
              backDir = dirCalculator.OppositeDirection(selected);   
            }
            //백트래킹 중이라면 이전 방향 제외 
            if(isBackTraking)
            {
              pathNodes[calY,calX].ChangeOpenStatus(backDir, CLOSED); 
            }
            //▼ 경로 시작 노드의 모든 면이 막혀있고 현재 좌표가 해당 좌표이거나 경로 시작 노드가 막혀있으면 실패로 간주 
            if(
                pathNodes[startPosition.y, startPosition.x + 1].IsBlocked || 
                (!pathNodes[startPosition.y,startPosition.x + 1].CheckEveryDirectionBlocked() && 
                calX == startPosition.x + 1 && calY == startPosition.y)
            ) 
            {
                isSuccess = false;
                return;
            }

            //▼ 시작 지점이라면 동쪽으로 고정 
            else if(calX == startPosition.x && calY == startPosition.y) 
            {
                selected = DIRECTION.East;
            }

            //▼ 목표 지점 바로 앞이라면 동쪽으로 고정
            else if(calX == destinationPosition.x - 1 && calY == destinationPosition.y)
            {
                selected = DIRECTION.East;
            }

            //▼ 열려있는 방향을 우선순위에 따라서 선택하기
            else
            {
                selected = DIRECTION.None;

                openDirection = pathNodes[calY,calX].GetAllOpenDirection();
                
                int selectIndex = int.MaxValue;

                for(int i = 0; i < 4; i++)
                {   
                    if(openDirection[i] == DIRECTION.None)
                        continue;

                    for(int j = 0; j < 4; j++)
                    {
                        if(j < selectIndex && currentDirPriority[j] == openDirection[i])
                        {
                            selected = openDirection[i];
                            selectIndex = j;  
                        }
                    }

                    if(selectIndex == 0)
                    {
                        break;
                    }
                }
            }
             foreach(var dir in tempClosedDirections)
                {
                    //▼ 백트래킹 중이라면 돌아가는 방향 영구적으로 닫기 
                    if(dir == backDir && isBackTraking)
                    {
                        continue;
                    }
                    //▼ 임시로 막아두었던 방향 열기  
                    else if(dir != DIRECTION.None)
                    { 
                        pathNodes[calY, calX].ChangeOpenStatus(dir, OPENED);  
                    } 
                    else
                    {
                    
                    }
                }

            //▼ 실제 이동한 경로 추가 하면서 해당 방향 막기 
            switch(selected)
            {
                case DIRECTION.East:
                        pathNodes[calY,calX].ChangeOpenStatus(selected, CLOSED);
                        isBackTraking = false;
                        calX++;
                        break;
                case DIRECTION.West:
                        pathNodes[calY,calX].ChangeOpenStatus(selected, CLOSED);
                        isBackTraking = false;
                        calX--;
                        break;
                case DIRECTION.South:
                        pathNodes[calY,calX].ChangeOpenStatus(selected, CLOSED);
                        isBackTraking = false;
                        calY--;
                        break;      
                case DIRECTION.North:
                        pathNodes[calY,calX].ChangeOpenStatus(selected, CLOSED);
                        isBackTraking = false;
                        calY++;
                        break;
                case DIRECTION.None:
                        isBackTraking = true;
                        break;
            }

            //▼ 백트래킹 중이 아니라면 현재 좌표를 경로에 저장 
            if(!isBackTraking)
            {
                passed.Push(new Vector2Int(calX,calY));
                
            }
            //▼ 백트래킹 중이라면 현재 되돌아가는 방향을 닫고 돌아가기 
            else 
            { 
                passed.Pop();
                Vector2Int temp = passed.Peek();
                if(temp.x ==  calX - 1 && temp.y == calY)
                {
                    selected = DIRECTION.West;
                }
                else if(temp.x ==  calX && temp.y == calY - 1)
                {
                   selected = DIRECTION.South; 
                }
                else if(temp.x ==  calX + 1 && temp.y == calY )
                {
                    selected = DIRECTION.East;                    
                }
                else if(temp.x ==  calX  && temp.y == calY +1)
                {
                    selected = DIRECTION.North;
                }
                else
                {
                    Debug.LogError("Unvaliable BackTracking Direction");
                    selected = DIRECTION.None;
                }
                pathNodes[calY,calX].ChangeOpenStatus(selected, CLOSED);
                
                calX = temp.x;
                calY = temp.y;
            }

           

            tempClosedDirections = new DIRECTION[]{DIRECTION.None,DIRECTION.None,DIRECTION.None,DIRECTION.None};

            //▼ 도착했다면 성공으로 전환한 뒤 탈출 
            if(calX == destinationPosition.x && calY == destinationPosition.y)
            {
                isSuccess = true;
                break;
            }
        }

        //▼ 성공했다면 path에 스택의 요소들 담기 
        if(isSuccess)
        {
            path = new Vector2Int[passed.Count];

            for(int i = passed.Count - 1; passed.Count > 0; i--)
            {
               Vector2Int pathVector = passed.Pop();
               Debug.Log($"{pathVector.x},{pathVector.y}");
               path[i] = pathVector;
            }
            
        }
         
    }
}
