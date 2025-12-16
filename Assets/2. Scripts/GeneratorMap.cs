using UnityEngine;

public class GeneratorMap : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject roadPrefab;
    public GameObject enemyBasePrefab;
    public GameObject allyBasePrefab;

    public const int CELL_SIZE = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < TileManager.MAP_SIZE_X; i++)
        {
            for (int j = 0; j < TileManager.MAP_SIZE_Y; j++)
            {
                TileData tile = TileManager.Instance.GetTileData(i, j);
                int x = tile.X * CELL_SIZE;
                int y = tile.Y * CELL_SIZE;

                GameObject tilePrefab = null;
                switch (tile.Type)
                {
                    case TileData.TYPE.None:
                    case TileData.TYPE.Base:
                    case TileData.TYPE.Wall:
                        tilePrefab = wallPrefab;
                        break;
                    case TileData.TYPE.Road:
                        tilePrefab = roadPrefab;
                        break;
                    case TileData.TYPE.AllyBase:
                        tilePrefab = allyBasePrefab;
                        break;
                    case TileData.TYPE.EnemyBase:
                        tilePrefab = enemyBasePrefab;
                        break;
                }

                Vector3 worldPosition = new Vector3(x, 0, y);
                if (tilePrefab != null)
                {
                    GameObject tileObject = Instantiate(tilePrefab, worldPosition, Quaternion.identity);
                    tileObject.transform.SetParent(transform);
                }

            }
        }
    }

}
