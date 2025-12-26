using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class GeneratorMap : MonoBehaviour
{
    public GameObject enemyBasePrefab;
    public GameObject allyBasePrefab;
    public GameObject transitionPrefab;

    public Dictionary<(int x, int y), TileInteractor> tiles = new Dictionary<(int x, int y), TileInteractor>();

    public void Generator()
    {
        for (int i = 0; i < TileManager.MAP_SIZE_Y; i++)
        {
            for (int j = 0; j < TileManager.MAP_SIZE_X; j++)
            {
                TileInfo tile = TileManager.Instance.GetTileInfo(j, i);
                int x = j * TileManager.CELL_SIZE;
                int y = i * TileManager.CELL_SIZE;

                GameObject tilePrefab = null;
                switch (tile.Type)
                {
                    case TileInfo.TYPE.Wall:
                    case TileInfo.TYPE.Road:
                        tilePrefab = transitionPrefab;
                        break;
                    case TileInfo.TYPE.AllyBase:
                        tilePrefab = allyBasePrefab;
                        break;
                    case TileInfo.TYPE.EnemyBase:
                        tilePrefab = enemyBasePrefab;
                        break;
                }

                Vector3 worldPosition = new Vector3(x, 0, y);
                if (tilePrefab != null)
                {
                    GameObject tileObject = Instantiate(tilePrefab, worldPosition, Quaternion.identity);
                    tileObject.transform.SetParent(transform);

                    TileInteractor interactor = tileObject.GetComponent<TileInteractor>();
                    interactor.Setup(j, i,tile.Type);

                    tiles.Add((j, i), interactor);

                    TileInfo data = TileManager.Instance.GetTileInfo(j, i);

                    //타일이 벽이나 길이라면
                    if (data.Type == TileInfo.TYPE.Wall || data.Type == TileInfo.TYPE.Road) { 
                        bool isWall = tile.Type == TileInfo.TYPE.Wall ? true : false;

                        tileObject.transform.GetChild(0).gameObject.SetActive(isWall);
                        tileObject.transform.GetChild(1).gameObject.SetActive(!isWall);

                        data.IsTransition = true;
                    }

                }

            }
        }
    }

}
