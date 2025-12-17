using System.Collections;
using TreeEditor;
using UnityEngine;

public class MoveTest : MonoBehaviour
{
    public float moveSpeed;
    Vector2Int[] path;
    int currentIdx = 1;
    Vector3 lastDir;

    public Animator anim;

    public float rotSpeed = 30f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int x = TileManager.Instance.enemyBasePosition.x;
        int y = TileManager.Instance.enemyBasePosition.y;

        lastDir = transform.forward;

        path = SinglePathGenerator.Instance.GetCurrentPath();
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        while (currentIdx < path.Length)
        {
            Vector3 nextPos = TileManager.Instance.GetWorldPosition(path[currentIdx]);

            Vector3 moveDir = (nextPos - transform.position).normalized;
            if (moveDir != Vector3.zero)
            {
              DirectionRotate(moveDir);
            }

            while (Vector3.Distance(transform.position, nextPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    nextPos,
                    moveSpeed * Time.deltaTime
                );

                yield return null;
            }

            transform.position = nextPos;
            currentIdx++;
        }

        Debug.Log("경로 이동 완료!");
        // Destroy(gameObject);
    }

    void DirectionRotate(Vector3 current)
    {

        float turnAngle = Vector3.SignedAngle(lastDir, current, Vector3.up);

        //if (turnAngle > 45f)
        //{
        //    anim.SetTrigger("TurnRight");
        //}
        //else if (turnAngle < -45f)
        //{
        //    anim.SetTrigger("TurnLeft");
        //}
        //yield return new WaitForSeconds(0.1f);

        //AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        //yield return new WaitForSeconds(stateInfo.length);

        //StartCoroutine(RotateAfterAnimation(current));

        transform.rotation = Quaternion.LookRotation(current);
        //현재 바라보고 있는 방향
        lastDir = current;
    }

}
