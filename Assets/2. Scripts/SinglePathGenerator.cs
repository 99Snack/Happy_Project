using System;
using System.Collections.Generic;
using UnityEngine;


//▼ 방향을 나타내는 enum
enum Direction
{
    West = -2, South = 1, None = 0, North = -1, East = 2
}

//▼ 이전 스텝의 정보를 저장해두기 위한 구조체
struct Step
{
    public readonly int curveCount;
    public readonly int currentX;
    public readonly int currentY;
    public readonly int continuousYStep;
    public readonly int continuousCurve;
    public readonly Direction selectDirection;
    public readonly Direction lastDirection;
    public List<Direction> banDirections;
    
    public void AddBanDirectionList(List<Direction> directionList)
    {
        banDirections.AddRange(directionList);
    }
    public void AddBanDirection(Direction direction)
    {
        banDirections.Add(direction);
    }

    /// <summary>
    /// Step 생성자
    /// </summary>
    /// <param name="CurveCount">커브 누적횟수 </param>
    /// <param name="CurrentX">현재 X좌표 </param>
    /// <param name="CurrentY">현재 Y좌표</param>
    /// <param name="ContinuousYStep">Y 방향 연속 이동 횟수</param>
    /// <param name="ContinuousCurve">연속 커브 횟수</param>
    /// <param name="SelectDirection">다음 방향</param>
    /// <param name="LastDirection">이전 방향 </param>

    public Step(int CurveCount, int CurrentX, int CurrentY, int ContinuousYStep, int ContinuousCurve, Direction SelectDirection, Direction LastDirection)
    {
        curveCount = CurveCount;
        currentX = CurrentX;
        currentY = CurrentY;
        continuousYStep = ContinuousYStep;
        continuousCurve = ContinuousCurve;
        selectDirection = SelectDirection;
        banDirections = new();
        lastDirection = LastDirection;
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
    //▼ 싱글톤을 위한 변수 
    private static SinglePathGenerator instance;
    public static SinglePathGenerator Instance => instance;

    //▼ 14 X 8 길이의 타일
    [SerializeField] const int TILE_X_LENGTH = 14;
    [SerializeField] const int TILE_Y_LENGTH = 8;

    //▼ 계산된 단일 경로
    private Vector2Int[] singlePath;
    //▼ 적 베이스 캠프 중앙 위치 
    private Vector2Int startPos;
    //▼ 아군 베이스 캠프 중앙 위치
    private Vector2Int destinationPos;


    private const int MAX_CURVE_COUNT = 8; //최대 꺾임 수
    private int curveCount; //꺾임 수 
    
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
    /// (첫 경로 생성용) 경로가 저장된 배열을 따라 타일을 길로 변경해주는 메서드  
    /// </summary>
    public void GeneratePath()
    {
        ActiveFindPath();
        
        foreach(var path in singlePath)
        {   
            TileData.TYPE type = TileManager.Instance.GetTileType(path.x, path.y);
            if (type == TileData.TYPE.AllyBase || type == TileData.TYPE.EnemyBase) 
                continue;
            
            TileManager.Instance.SetTileData(path.x, path.y, TileData.TYPE.Road);
        }
    }

    /// <summary>
    /// 현재 생성되어 있는 경로를 반환해주는 메서드 
    /// </summary>
    /// <returns>//현재 생성되어 있는 경로 </returns>
    public Vector2Int[] GetCurrentPath()
    {
        return singlePath;
    }

    /// <summary>
    /// 길찾기를 실행하는 메서드 
    /// </summary>
    private void ActiveFindPath()
    {   
        Init();
        FindPath();
    }

    /// <summary>
    /// 초기화 메서드 
    /// </summary>
    private void Init()
    {
        curveCount = 0;
        startPos = TileManager.Instance.enemyBasePosition;
        destinationPos = TileManager.Instance.allyBasePosition;
    }

    /// <summary>
    /// 반대 방향을 반환하는 메서드  
    /// </summary>
    /// <param name="dir">원하는 방향</param>
    /// <returns></returns>
    private Direction OppositeDirection(Direction dir)
    {
        switch (dir)
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

        for (int i = 0; i < PathList.Count; i++)
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

        switch (dir)
        {
            case Direction.North:
                currentY += 1;
                break;
            case Direction.West:
                currentX -= 1;
                break;
            case Direction.South:
                currentY -= 1;
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
    private Vector2Int PredictNextPosition(Direction dir, int currentX, int currentY)
    {
        switch (dir)
        {
            case Direction.North:
                currentY += 1;
                break;
            case Direction.West:
                currentX -= 1;
                break;
            case Direction.South:
                currentY -= 1;
                break;
            case Direction.East:
                currentX += 1;
                break;
        }
        Vector2Int nextPosition = new Vector2Int(currentX, currentY);
        return nextPosition;
    }

    /// <summary>
    /// 방향을 정해주는 메서드 
    /// </summary>
    /// <param name="lastDirection">바로 직전의 방향</param>
    /// <param name="banDirection">루트가 없는 방향</param>
    /// <param name="takenPath">지나간 좌표</param>
    /// <param name="continuousYStep">연속된 Y방향 변화 수 </param>
    /// <param name="continuousCurve">연속된 방향 전환 수 </param>
    /// <param name="currentX">현재 진행된 X좌표 </param>
    /// <param name="currentY">현재 진행된 Y좌표</param>
    /// <param name="currentPathLength">현재까지 진행된 경로의 길이 </param>
    /// <returns></returns>
    private Direction SelectDirection(Direction lastDirection, List<Direction> banDirection, List<Vector2Int> takenPath, int continuousYStep, int continuousCurve, int currentX, int currentY, int currentPathLength)
    {
        //▼ 가능한 방향 리스트 
        List<Direction> valiableDirList = new() { Direction.East, Direction.West, Direction.North, Direction.South };

        //▼ 다음 포지션
        Vector2Int nextPosition;

        //▼ 금지된 방향은 제거 
        if (banDirection.Count > 0)
        {
            foreach (var dir in banDirection)
            {
                valiableDirList.Remove(dir);
            }
        }

        //▼ curveCount에 여유가 없거나 연속된 Y방향이 3번이라면 Y방향은 제외 
        if (curveCount == MAX_CURVE_COUNT || continuousYStep == 3)
        {
            valiableDirList.Remove(Direction.North);
            valiableDirList.Remove(Direction.South);
        }

        for (int i = valiableDirList.Count - 1; i >= 0; i--)
        {
            nextPosition = PredictNextPosition(valiableDirList[i], currentX, currentY);

            if (takenPath.Contains(nextPosition))
            {
                valiableDirList.Remove(valiableDirList[i]);
                continue;
            }

            //▼ 방향을 적용한 다음 위치가 범위를 벗어나면 
            if (nextPosition.x > TILE_X_LENGTH - 1 || nextPosition.y > TILE_Y_LENGTH - 1 || nextPosition.x < 0 || nextPosition.y < 0)
            {
                valiableDirList.Remove(valiableDirList[i]);
                continue;
            }
            //▼ 반대 방향 제외
            else if (OppositeDirection(valiableDirList[i]) == lastDirection)
            {
                valiableDirList.Remove(valiableDirList[i]);
                continue;
            }
            //▼ 꺾임 수 제한 확인  
            else if (curveCount >= MAX_CURVE_COUNT && Math.Abs((int)valiableDirList[i]) != Math.Abs((int)lastDirection))
            {
                valiableDirList.Remove(valiableDirList[i]);
                continue;
            }
            //▼ 해당 방향 진행시 최대 길이 및 최소 길이  초과여부 확인
            else if (CalculateBasePath(valiableDirList[i], currentX, currentY) + 1 + currentPathLength > 24 || CalculateBasePath(valiableDirList[i], currentX, currentY) + 1 + currentPathLength < 16)
            {
                valiableDirList.Remove(valiableDirList[i]);
                continue;
            }
        }
        //▼ 방향이 없다면 에러 출력 
        if (valiableDirList.Count == 0)
        {
            return Direction.None;
        }
        //▼ 2개 이상이면 랜덤으로 선택 
        if (valiableDirList.Count > 1)
        {
            //▼ 직전에 커브했다면 커브를 최대한 피한다.
            if (continuousCurve > 0 && valiableDirList.Contains(lastDirection))
                return lastDirection;

            else
                return valiableDirList[UnityEngine.Random.Range(0, valiableDirList.Count)];
        }
        else
        {
            //▼ 1개라면 바로 출력 
            return valiableDirList[0];
        }
    }

    /// <summary>
    /// 단일 경로를 찾는 메서드
    /// </summary>
    private void FindPath()
    {
        //▼ 이전 방향 
        Direction lastDirection = Direction.None;

        //▼ 선택된 방향
        Direction selectedDirection;

        //▼ 경로 추가용 벡터
        Vector2Int pathVector = new Vector2Int(startPos.x, startPos.y);

        //▼ 조건 확인용 좌표
        int currentX = startPos.x;
        int currentY = startPos.y;

        //▼ 연속된 Y 수
        int continuousYStep = 0;

        //▼ 연속된 curve 수 
        int continuousCurve = 0;

        //▼ 금지된 방향
        List<Direction> banDirection = new();

        //▼ 어느 방향으로 추가할지를 미리 정의 해둔 리스트
        List<Direction> AddDirection = new();

        //▼ 지나간 경로를 저장하는 리스트 
        List<Vector2Int> takenPath = new();

        //▼ 실제 경로를 저장해놓은 리스트  
        List<Vector2Int> path = new();

        //▼ 스텝을 저장해놓은 스택
        Stack<Step> steps = new();
      

        //▼ 적군 베이스 캠프 경로에서 제외
        takenPath.Add(new Vector2Int(currentX, currentY));
        takenPath.Add(new Vector2Int(currentX, currentY + 1));
        takenPath.Add(new Vector2Int(currentX, currentY - 1));

        //▼ 저장된 currentX, currentY가 타겟좌표에 도달할 때까지 
        while (true)
        {
            //▼ X좌표가 목적지 바로 앞이고 Y좌표는 맞춰줬을 때 
            if (currentX == destinationPos.x - 1 && currentY == destinationPos.y)
            {
                break;
            }
            else
            {
                //▼ 첫 방향은 East로 고정 
                if (AddDirection.Count <= 0)
                {
                    AddDirection.Add(Direction.East);
                    currentX += 1;
                    lastDirection = Direction.East;
                    steps.Push(new Step(curveCount, currentX, currentY, continuousYStep, continuousCurve, Direction.East, lastDirection));
                    takenPath.Add(new Vector2Int(currentX, currentY));
                    continue;
                }

                //▼ 가능한 방향을 저장
                selectedDirection = SelectDirection(lastDirection, banDirection, takenPath, continuousYStep, continuousCurve, currentX, currentY, AddDirection.Count + 1);

                //▼ 가능한 경로가 없을경우 재탐색 
                if (selectedDirection == Direction.None)
                {
                    if (steps.Count <= 0)
                    {
                        Debug.LogError("InfiniteLoop");
                        break;
                    }
                    if(AddDirection.Count - 1 < 0)
                    {
                        Debug.LogError("Too Low Count");
                        break;
                    }

                    AddDirection.RemoveAt(AddDirection.Count - 1);
                    
                    Step lastStep = steps.Pop();

                    if (banDirection.Count > 0)
                    {
                        banDirection.Clear();
                    }

                    takenPath.Remove(new Vector2Int(currentX, currentY));
                    
                    currentX = lastStep.currentX;
                    currentY = lastStep.currentY;

                    continuousYStep = lastStep.continuousYStep;
                    continuousCurve = lastStep.continuousCurve;

                    curveCount = lastStep.curveCount;
                    lastStep.AddBanDirection(lastStep.selectDirection);
                    banDirection.AddRange(lastStep.banDirections);

                    lastDirection = lastStep.lastDirection;
                    continue;
                }

                AddDirection.Add(selectedDirection);

                //▼ 이전 방향과 다르다면 
                if (Math.Abs((int)lastDirection) != Math.Abs((int)selectedDirection))
                {
                    curveCount++;
                    continuousCurve++;
                }
                //▼ 이전 방향과 같다면
                else
                {
                    continuousCurve = 0;
                }

                //▼ 스탭 저장 
                Step curStep = new Step(curveCount, currentX, currentY, continuousYStep, continuousCurve, selectedDirection, lastDirection);

                if (banDirection.Count > 0)
                {
                    curStep.AddBanDirectionList(banDirection);
                    banDirection.Clear();
                }

                steps.Push(curStep);

                //▼ 좌표 값 바꾸기                            
                switch (selectedDirection)
                {
                    case Direction.West:
                        currentX -= 1;
                        continuousYStep = 0;
                        break;
                    case Direction.North:
                        currentY += 1;
                        continuousYStep++;
                        break;
                    case Direction.South:
                        currentY -= 1;
                        continuousYStep += 1;
                        break;
                    case Direction.East:
                        currentX += 1;
                        continuousYStep = 0;
                        break;
                }

                lastDirection = selectedDirection;

                takenPath.Add(new Vector2Int(currentX, currentY));
            }
        }
        //▼ 종료 시 방향 East로 고정
        AddDirection.Add(Direction.East);
        //▼ 시작 위치 추가 
        path.Add(pathVector);
        //▼ 실제로 path에 벡터를 추가 
        foreach (var dir in AddDirection)
        {
            switch (dir)
            {
                case Direction.West:
                    pathVector.x -= 1;
                    break;
                case Direction.North:
                    pathVector.y += 1;
                    break;
                case Direction.South:
                    pathVector.y -= 1;
                    break;
                case Direction.East:
                    pathVector.x += 1;
                    break;
            }
            path.Add(pathVector);
        }

        CopyPathListToArray(path);
    }
}










