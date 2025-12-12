using System.Collections.Generic;
using System.Text;
using UnityEngine;


/// <summary>
/// 게임 시작시 단일경로 생성하는 컴포넌트 
/// 제약조건:  Path의 Length: 16이상 ~ 24이하
///            Curve 최대 8회
///            y는 최대 연속 3칸 
/// </summary>

public class SinglePathGenerator : MonoBehaviour
{
    //▼ 14 X 8 길이의 타일
    int tileXLength = 14;
    int tileYLength = 8;

    int [ , ] Tile;
    //▼ 계산된 단일 경로
    Vector2Int[] SinglePath;
    //▼ 적 베이스 캠프 위치 
    Vector2Int EnemyBaseCampPos;
    //▼ 아군 베이스 캠프 위치
    Vector2Int AllyBaseCampPos; 

    int curveCount; //꺾임 수 

    StringBuilder testSB; //테스트 용 스트링빌더 

    /// <summary>
    /// 초기화 메서드 
    /// </summary>
    private void Init()
    {
        Tile = new int[tileYLength, tileXLength];
        curveCount = 0;
    }

    /// <summary>
    /// Test 결과를 디버그 로그를 통해 보여주는 임시 메서드
    /// </summary>
    private void PrintTestResult()
    {
        testSB = new StringBuilder();

        for(int i = 0; i < SinglePath.Length; i++)
        {
            testSB.Append((SinglePath[i].x, SinglePath[i].y) );
        }
        Debug.Log(testSB.ToString()); 
    }

    /// <summary>
    /// 실제로 경로를 찾는 메서드 
    /// </summary>
    private void FindPath()
    {
        //todo: x와 y가 원하는 좌표가 될 때까지 둘 중 하나를 랜덤으로 add한다 y좌표를 add하는 것은 최대 3회를 넘기지 않는다 
        //리스트를 통해 경로를 저장 하고 이를 뒤집는 방식으로 만약 x를 추가하다가 y를 추가하는 메서드로 변경되면 curveCount가 증가한 것으로 간주한다.
        
        const bool X = true;
        const bool Y = false;
        
        Vector2Int curVector = new Vector2Int(EnemyBaseCampPos.x, EnemyBaseCampPos.y);
        int curx = EnemyBaseCampPos.x;
        int cury = EnemyBaseCampPos.y;
        int consecutiveYCount = 0; //연 이어진 Y 카운트 
        
        //▼ 어느 방향으로 추가할지를 미리 정의 해둔 리스트
        List<bool> AddDirection = new(); 
        
        //▼ 실제 경로를 저장해놓은 리스트  
        List<Vector2Int> Path = new();  
        
        while(curx == AllyBaseCampPos.x && cury == AllyBaseCampPos.y)
        {
            if(AddDirection.Count > 2 && consecutiveYCount == 3 )// 계산된 경로가 2보다 크고 y가 3회 연속되었을때 X로 설정후 카운트 초기화 
            {
                AddDirection.Add(X);
                curx++;
                consecutiveYCount = 0;
                continue;
            }
            
            bool temp = RandomBool();
            //만약 꺾임 수가 8회 이상이라면 Random을 하지 않고 그 상태에서 직진하는데 만약 x랑 y좌표 둘다 조건을 만족하지 않으면 yCount를 해치지 않고 기존의 
            if(temp)
            {
                AddDirection.Add(X);
                curx++;
                consecutiveYCount = 0;
            }
            else
            {
                AddDirection.Add(Y);
                cury++;
                consecutiveYCount++;     
            }
        }

        foreach (bool dirX in AddDirection)
        {
            if(dirX)
            {
               curVector.x++;
            }
            else
            {
                curVector.y++;
            }
        }

    }

    private void GenerateCurveOnPath()
    {
        //todo: 최소 타일 수에 맞지 않는 경로를 조건을 지켜 랜덤으로 커브를 만드는 함수 
    }
    private void DeleteCurveOnPath()
    {
         
    }

    private bool RandomBool()
    {
        bool result = Random.Range(0,2) == 1;
        return result;
    }

}










