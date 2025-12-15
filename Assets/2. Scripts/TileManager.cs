using UnityEngine;

public class TileManager : MonoBehaviour
{
    private static TileManager instance;
    public static TileManager Instance => instance;

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

    //모든 타일들 관리
    public TileData[,] allTiles;

    // 맵 사이즈
    public const int MAP_SIZE_X = 14;
    public const int MAP_SIZE_Y = 8;

    public const int CELL_SIZE = 2;

    //캠프 좌표 관리 y 1~6 랜덤 좌표
    const int enemyBaseCoordX = 0;
    const int allyBaseCoordX = 13;

    public (int x, int y) enemyBasePosition { get; private set; }
    public (int x, int y) allyBasePosition { get; private set; }

    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        // 맵 초기화
        allTiles = new TileData[MAP_SIZE_Y, MAP_SIZE_X];

        for (int i = 0; i < MAP_SIZE_Y; i++)
        {
            for (int j = 0; j < MAP_SIZE_X; j++)
            {
                allTiles[i, j] = new TileData(j,i,TileData.TYPE.None);
            }
        }

        //베이스 랜덤 
        RandomBaseCamp();

        //타일 맵 생성
        GeneratorMap map = FindAnyObjectByType<GeneratorMap>();
        if (map != null)
        {
            map.Generator();
        }
    }

    void RandomBaseCamp()
    {
        //랜덤 베이스 위치 정하기
        int randomEnemyBaseCoordY = Random.Range(1, MAP_SIZE_Y - 1);
        int randomAllyBaseCoordY = Random.Range(1, MAP_SIZE_Y - 1);

        enemyBasePosition = (enemyBaseCoordX, randomEnemyBaseCoordY);
        allyBasePosition = (allyBaseCoordX, randomAllyBaseCoordY);

        //정해진 좌표에 베이스 설정
        allTiles[enemyBasePosition.y - 1, enemyBasePosition.x].Type = TileData.TYPE.EnemyBase;
        allTiles[enemyBasePosition.y, enemyBasePosition.x].Type = TileData.TYPE.EnemyBase;
        allTiles[enemyBasePosition.y + 1, enemyBasePosition.x].Type = TileData.TYPE.EnemyBase;

        allTiles[allyBasePosition.y - 1, allyBasePosition.x].Type = TileData.TYPE.AllyBase;
        allTiles[allyBasePosition.y, allyBasePosition.x].Type = TileData.TYPE.AllyBase;
        allTiles[allyBasePosition.y + 1, allyBasePosition.x].Type = TileData.TYPE.AllyBase;
    }

    public void SetTileData(int x, int y, TileData.TYPE type)
    {
        if (type != TileData.TYPE.Wall && type != TileData.TYPE.Road) return;

        if (IsValidCoordinate(x, y))
        {
            allTiles[y, x] = new TileData(x,y,type);
        }
    }

    public TileData.TYPE GetTileType(int x, int y)
    {

        if (IsValidCoordinate(x, y))
        {
            return allTiles[y, x].Type;
        }


        return TileData.TYPE.None;
    }
    public TileData GetTileData(int x, int y)
    {

        if (IsValidCoordinate(x, y))
        {
            return allTiles[y, x];
        }


        return null;
    }

    //좌표가 유효한지
    bool IsValidCoordinate(int x, int y)
    {
        if (x >= 0 && x < MAP_SIZE_X && y >= 0 && y < MAP_SIZE_Y)
        {
            return true;
        }
        else
        {
            Debug.LogError($"유효하지 않은 좌표입니다({x},{y})");
        }

        return false;
    }
}

