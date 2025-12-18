using System.Collections.Generic;
using Unity.Multiplayer.Center.Common.Analytics;
using UnityEngine;

public class GeneratorMap : MonoBehaviour
{
    public GameObject enemyBasePrefab;
    public GameObject allyBasePrefab;
    public GameObject transitionPrefab;

    public Dictionary<(int x, int y), GameObject> tiles = new Dictionary<(int x, int y), GameObject>();

    public void Generator()
    {
        for (int i = 0; i < TileManager.MAP_SIZE_Y; i++)
        {
            for (int j = 0; j < TileManager.MAP_SIZE_X; j++)
            {
                TileData tile = TileManager.Instance.GetTileData(j, i);
                int x = j * TileManager.CELL_SIZE;
                int y = i * TileManager.CELL_SIZE;

                GameObject tilePrefab = null;
                switch (tile.Type)
                {
                    case TileData.TYPE.Wall:
                    case TileData.TYPE.Road:
                        tilePrefab = transitionPrefab;
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
                    tiles.Add((j, i), tileObject);

                    TileInteractor interactor = tileObject.GetComponent<TileInteractor>();
                    interactor.Setup(j, i,tile.Type);

                    TileData data = TileManager.Instance.GetTileData(j, i);

                    //타일이 벽이나 길이라면
                    if (data.Type == TileData.TYPE.Wall || data.Type == TileData.TYPE.Road) { 
                        bool isWall = tile.Type == TileData.TYPE.Wall ? true : false;

                        tileObject.transform.GetChild(0).gameObject.SetActive(isWall);
                        tileObject.transform.GetChild(1).gameObject.SetActive(!isWall);

                        data.IsTransition = true;
                    }

                }

            }
        }
    }

}
