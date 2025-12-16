using System.Collections;
using UnityEngine;

public class MoveTest : MonoBehaviour
{
    public float moveSpeed;
    Vector2Int[] path;
    int currentIdx = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int x = TileManager.Instance.enemyBasePosition.x;
        int y = TileManager.Instance.enemyBasePosition.y;

        path = SinglePathGenerator.Instance.GetCurrentPath();
        Debug.Log(path.Length);
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        while (currentIdx < path.Length)
        {
            Vector3 nextPos = TileManager.Instance.GetWorldPosition(path[currentIdx]);

            while (Vector3.Distance(transform.position, nextPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    nextPos,
                    moveSpeed * Time.deltaTime
                );

                yield return null;
            }

            Debug.Log(path[currentIdx]);

            //다음 경로
            transform.position = nextPos;

            currentIdx++;

        }

        Debug.Log("경로 이동 완료!");
        // Destroy(gameObject);
    }
}
