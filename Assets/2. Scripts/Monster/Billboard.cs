using UnityEngine;

// 항상 카메라만 바라보게 하는 빌보드 스크립트
public class Billboard : MonoBehaviour
{
    Camera mainCam;
    void Start()
    {
        mainCam = Camera.main;
    }
    void LateUpdate()
    {
       transform.forward = mainCam.transform.forward;
    }
}
