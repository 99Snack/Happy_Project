/*
    실제 경로를 생성하는 클래스 
*/
using UnityEngine;

public class PlayPathGenerator 
{
    //▼ 타일의 X와 Y 길이 상수
    private const int TILE_X_LENGTH = 14;
    private const int TILE_Y_LENGTH = 8;
    //▼ 적군 베이스 캠프 위치
    private Vector2Int startPosition; 
    //▼ 아군 베이스 캠프 위치
    private Vector2Int destinationPosition;
    //▼ 경로 
    private Vector2Int[] path;
    //▼ 규칙 번호 
    private int ruleNumber;
    //▼ 경로 타일 정보 배열
    private PathNodeData[] pathNodes;
    //▼ 지나간 경로를 저장하는 리스트 todo: Vector2Int로 할지 PathNodeData로 할지 고민 
    //private List<PathNodeData>;

    private void Init()
    {
        startPosition = TileManager.Instance.enemyBasePosition;
        destinationPosition = TileManager.Instance.allyBasePosition;
        //ruleNumber = 스폰번호 % 4;
        //pathNodes= ????

    }
}
