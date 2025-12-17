using UnityEngine;

public class TestBullet : MonoBehaviour
{
    public float MoveSpeed = 10f; 

   void Update()
    {
        transform.Translate(Vector3.forward * MoveSpeed * Time.deltaTime);
    }
}
