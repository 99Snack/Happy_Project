using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine;

//반대 방향은 양수 음수가 반대 
enum Direction
{
   West = -2, South = 1, None = 0, North = -1, East = 2 
}

readonly struct Step
{
    public readonly int curveCount;
    public readonly int currentX;
    public readonly int currentY; 
    public readonly int continuousYStep;
    public readonly Direction direction;
    /// <summary>
    /// Step 생성자
    /// </summary>
    /// <param name="CurveCount"></param>
    /// <param name="CurrentX"></param>
    /// <param name="CurrentY"></param>
    /// <param name="ContinuousYStep"></param>
    /// <param name="SelectDirection"></param>

    public Step(int CurveCount, int CurrentX, int CurrentY, int ContinuousYStep, Direction SelectDirection)
    {
        curveCount = CurveCount;
        currentX = CurrentX;
        currentY = CurrentY;
        continuousYStep = ContinuousYStep;
        direction = SelectDirection;
    }
}

/// <summary>
/// 게임 시작시 단일경로 생성하는 컴포넌트 
/// 제약조건:  Path의 Length: 16이상 ~ 24이하
///            Curve 최대 8회
///            y는 최대 연속 3칸 
/// </summary>

public class SinglePathGenerator : MonoBehaviour
{
    //▼ 14 X 8 길이의 타일
    [SerializeField] int tileXLength = 14;
    [SerializeField] int tileYLength = 8;
     
    //▼ 계산된 단일 경로
    Vector2Int[] singlePath;
    //▼ 적 베이스 캠프 중앙 위치 
    Vector2Int startPos;
    //▼ 아군 베이스 캠프 중앙 위치
    Vector2Int destinationPos; 

    int curveCount; //꺾임 수 
    int maxCurveCount = 8; //최대 꺾임 수

    StringBuilder testSB; //테스트 용 스트링빌더 
    Coroutine pathCoroutine; //

    public void buttonPressed()
    {
        if(pathCoroutine == null)
        {
            Init();
            StartCoroutine(FindPath());
            PrintTestResult();
        }
    } 
    /// <summary>
    /// 초기화 메서드 
    /// </summary>
    private void Init()
    {
        curveCount = 0;
        startPos = new Vector2Int(0,3);
        destinationPos = new Vector2Int(13,6);
    }
    /// <summary>
    /// Test 결과를 디버그 로그를 통해 보여주는 임시 메서드
    /// </summary>
    private void PrintTestResult()
    {
        testSB = new StringBuilder();

        for(int i = 0; i < singlePath.Length; i++)
        {
            testSB.Append((singlePath[i].x, singlePath[i].y) );
        }
        Debug.Log(testSB.ToString()); 
    }
    private Direction OppositeDirection(Direction dir)
    {
        switch(dir)
        {
             case Direction.North:
                return Direction.South;

            case Direction.West:
                return Direction.East;
                
            case Direction.South:
                return Direction.North;
               
            case Direction.East:
                return Direction.West;
            default:
                break;       
        }
        Debug.LogError("Not valiable direction to find opposite direction");
        return Direction.None;
    }
    /// <summary>
    /// List를 실제 Path Array에 복사 
    /// </summary>
    /// <param name="PathList">복사할 리스트 </param>
    public void CopyPathListToArray(List<Vector2Int> PathList)
    {
        singlePath = new Vector2Int[PathList.Count];
        
        for(int i = 0; i < PathList.Count; i++)
        {
            singlePath[i] = PathList[i];
        }
    }

    /// <summary>
    /// 원하는 방향으로 갔을 때  계산한 길이 경로 수 반환하는 메서드
    /// </summary>
    /// <param name="dir">추가할 방향</param>
    /// <param name="currentX">현재 X좌표 </param>
    /// <param name="currentY">현재 Y좌표 </param>
    /// <returns></returns>
     
    private int CalculateBasePath(Direction dir, int currentX, int currentY)
    {
        int result = 0;

        switch(dir)
        {
            case Direction.North:
                currentY -= 1;
                break;
            case Direction.West:
                currentX -= 1;
                break;
            case Direction.South:
                currentY += 1;
                break;
            case Direction.East:
                currentX += 1;
                break;
        }
        result = Math.Abs(destinationPos.x - currentX) + Math.Abs(destinationPos.y - currentY);
        return result;
    }

    /// <summary>
    /// 해당 방향으로 갔을 때 다음의 위치를 예측하는 메서드
    /// </summary>
    /// <param name="dir">원하는 방향</param>
    /// <param name="currentX">현재 x좌표</param>
    /// <param name="currentY">현재 y좌표 </param>
    /// <returns></returns>
    private Vector2 PredictNextPosition(Direction dir, int currentX, int currentY)
    {
        switch(dir)
        {
            case Direction.North:
                currentY -= 1;
                break;
            case Direction.West:
                currentX -= 1;
                break;
            case Direction.South:
                currentY += 1;
                break;
            case Direction.East:
                currentX += 1;
                break;
        }
        Vector2 nextPosition = new Vector2(currentX, currentY);
        return nextPosition;
    }

    /// <summary>
    /// 방향을 정해주는 메서드 
    /// </summary>
    /// <param name="lastDirection">바로 직전의 방향</param>
    /// <param name="banDirection">루트가 없는 방향</param>
    /// <param name="continuousYStep">연속된 Y방향 변화 수 </param>
    /// <param name="currentX">현재 진행된 X좌표 </param>
    /// <param name="currentY">현재 진행된 Y좌표</param>
    /// <param name="currentPathLength">현재까지 진행된 경로의 길이 </param>
    /// <returns></returns>
    private Direction SelectDirection(Direction lastDirection, Direction banDirection, int continuousYStep, int currentX, int currentY, int currentPathLength)
    {
        //▼ 가능한 방향 리스트 
        List<Direction> valiableDirList = new() {Direction.East, Direction.West, Direction.North, Direction.South};

        //▼ 다음 포지션
        Vector2 nextPosition;
        
        //▼ 금지된 방향은 제거 
        if(banDirection != Direction.None)
        {
            valiableDirList.Remove(banDirection);
        }
        //▼ curveCount에 여유가 없거나 연속된 Y방향이 3번이라면 Y방향은 제외 
        if(curveCount == maxCurveCount || continuousYStep == 3)
            {
                valiableDirList.Remove(Direction.North);
                valiableDirList.Remove(Direction.South);
            }

        for(int i = valiableDirList.Count - 1; i >= 0; i--)
        {   
            nextPosition = PredictNextPosition(valiableDirList[i], currentX, currentY);

            //▼ 방향을 적용한 다음 위치가 범위를 벗어나면 
            if(nextPosition.x > tileXLength - 1 || nextPosition.y > tileYLength - 1 || nextPosition.x < 0 || nextPosition.y < 0 )
            {
                valiableDirList.Remove(valiableDirList[i]);
                continue;
            }
            //▼ 반대 방향 제외
            else if(OppositeDirection(valiableDirList[i]) == lastDirection)
            {
                valiableDirList.Remove(valiableDirList[i]);
                continue;
            }
            //▼ 꺾임 수 제한 확인  
            else if(curveCount >= maxCurveCount && Math.Abs((int)valiableDirList[i]) != Math.Abs((int)lastDirection))
            {
                valiableDirList.Remove(valiableDirList[i]);
                continue;
            }
            //▼ 해당 방향 진행시 최대 길이 및 최소 길이  초과여부 확인
            else if(CalculateBasePath(valiableDirList[i], currentX, currentY) + 1 + currentPathLength > 24 || CalculateBasePath(valiableDirList[i], currentX, currentY) + 1 + currentPathLength < 16 )
            {   
                valiableDirList.Remove(valiableDirList[i]);
                continue;
            }
        }
        //방향이 없다면 에러 출력 
        if(valiableDirList.Count == 0)
        {
            return Direction.None;
        } 
        //▼ 2개 이상이면 랜덤으로 선택 
        if(valiableDirList.Count > 1)
        {
            return valiableDirList[UnityEngine.Random.Range(0, valiableDirList.Count)];
        }
        else
        {
            //▼ 1개 라면 바로 출력 
            return valiableDirList[0];   
        }
    }
    /// <summary>
    /// 실제로 단일 경로를 찾는 메서드
    /// </summary>
    private IEnumerator FindPath()
    {
        //▼ 이전 방향 
        Direction lastDirection = new Direction();
        
        //▼ 선택된 방향
        Direction selectedDirection = new Direction();

        //▼ 금지된 방향
        Direction banDirection = new Direction();
        
        //▼ 경로 추가용 벡터
        Vector2Int pathVector = new Vector2Int(startPos.x, startPos.y);
        
        //▼ 조건 확인용 좌표
        int currentX = startPos.x;
        int currentY = startPos.y;
        
        //▼ 연속된 Y 수
        int continuousYStep = 0;  
        
        //▼ 어느 방향으로 추가할지를 미리 정의 해둔 리스트
        List<Direction> AddDirection = new(); 
        
        //▼ 실제 경로를 저장해놓은 리스트  
        List<Vector2Int> path = new();  
        
        //▼ 스텝을 저장해놓은 스택
        Stack<Step> steps = new();

        //▼ 저장된 currentX, currentY가 타겟좌표에 도달할 때까지 
        while(true)
        {
            //▼ X좌표가 목적지 바로 앞이고 Y좌표는 맞춰줬을 때 
            if(currentX == destinationPos.x - 1 && currentY == destinationPos.y)
            {
                testSB = new StringBuilder();

                for(int i = 0; i < AddDirection.Count; i++)
                {
                    testSB.Append(AddDirection[i]);
                }
                Debug.Log(testSB.ToString()); 
                Debug.Log($"{currentX},{currentY}");
                break;
            }
    
            else
            {
                //첫 방향은 East로 고정 
                if(AddDirection.Count <= 0)
                {
                    AddDirection.Add(Direction.East);
                    currentX++;
                    lastDirection = Direction.East;
                    steps.Push(new Step(curveCount, currentX, currentY, continuousYStep, Direction.East));
                    continue;
                }

                //▼ 가능한 방향을 저장
                selectedDirection = SelectDirection(lastDirection, banDirection, continuousYStep, currentX, currentY, AddDirection.Count + 1);

                //▼ 가능한 경로가 없을경우 재탐색 
                if (selectedDirection == Direction.None)
                {
                    banDirection = lastDirection;
                    AddDirection.RemoveAt(AddDirection.Count - 1);

                    Step lastStep = steps.Pop();

                    currentX = lastStep.currentX;
                    currentY = lastStep.currentY;
                    continuousYStep = lastStep.continuousYStep;
                    curveCount = lastStep.curveCount;

                    continue;
                }

                AddDirection.Add(selectedDirection);
                banDirection = Direction.None;

                //▼ 좌표 값 바꾸기
                switch (selectedDirection)
                {
                    case Direction.West:
                        currentX -= 1;
                        continuousYStep = 0;
                        break;
                    case Direction.North:
                        currentY -= 1;
                        continuousYStep++;
                        break;
                    case Direction.South:
                        currentY += 1;
                        continuousYStep++;
                        break;
                    case Direction.East:
                        currentX += 1;
                        continuousYStep = 0;
                        break;
                }
                //▼ 이전 방향과 다르다면 
                if (Math.Abs((int)lastDirection) != Math.Abs((int)selectedDirection))
                {
                    curveCount++;
                }

                steps.Push(new Step(curveCount, currentX, currentY, continuousYStep, selectedDirection));

                lastDirection = selectedDirection;
            }
        } 
        //종료 시 방향 East로 고정
        AddDirection.Add(Direction.East);

        //실제로 path에 벡터를 추가 
        foreach (var dir in AddDirection)
        {
            switch(dir)
            {
                case Direction.West:
                    pathVector.x -= 1;
                    break;
                case Direction.North:
                    pathVector.y -= 1;
                    break;
                case Direction.South:
                    pathVector.y += 1;
                    break;
                case Direction.East:
                    pathVector.x += 1;
                    break;
            }
            path.Add(pathVector);
        }  
        
        CopyPathListToArray(path);
        yield return null;
        pathCoroutine = null;
    } 
}










