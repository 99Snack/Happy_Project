/*
    경로 생성의 전반적인 로직을 관리하는 클래스 
*/

using UnityEngine;

public class PathNodeManager : MonoBehaviour
{   
    //▼ 타일의 X와 Y 길이 상수
    private const int TILE_X_LENGTH = 14;
    private const int TILE_Y_LENGTH = 8;    
    private static PathNodeManager instance;
    private static PathNodeManager Instance => instance;
    //▼ 실제 PathNodeData를 담고 잇는 배열  
    PathNodeData[ , ] pathNodeTiles;
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

    private void GeneratPathNodeTiles()
    {   
        pathNodeTiles = new PathNodeData[TILE_Y_LENGTH,TILE_X_LENGTH];

        for (int i = 0; i < TILE_X_LENGTH; i++)
        {
            for(int j = 0; j < TILE_Y_LENGTH; j++ )
            {
                pathNodeTiles[j, i] = new PathNodeData(i, j); 
            }
        }
    }
}
