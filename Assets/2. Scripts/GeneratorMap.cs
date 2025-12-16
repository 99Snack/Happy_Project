using System.Collections.Generic;
using UnityEngine;

public class GeneratorMap : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject roadPrefab;
    public GameObject enemyBasePrefab;
    public GameObject allyBasePrefab;

    public Dictionary<Vector2Int, GameObject> tiles = new Dictionary<Vector2Int, GameObject>();

    public void Generator()
    {
        for (int i = 0; i < TileManager.MAP_SIZE_Y; i++)
        {
            for (int j = 0; j < TileManager.MAP_SIZE_X; j++)
            {
                TileData tile = TileManager.Instance.GetTileData(j,i);
                int x = j * TileManager.CELL_SIZE;
                int y = i * TileManager.CELL_SIZE;

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
                    tiles.Add(new Vector2Int(j,i), tileObject);

                    //todo : 에디터에서 직접 넣어줄지말지
                    //TileInteractor interactor = tileObject.AddComponent<TileInteractor>();
                    //interactor.Setup(i, j, tile.Type);
                }

            }
        }
    }

}
