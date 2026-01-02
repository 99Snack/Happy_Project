using UnityEngine;
using DG.Tweening;

public class ScaleAnim : MonoBehaviour
{
    [SerializeField] private Vector3 targetScale = new Vector3(0.2f, 0.2f, 0.2f);
    private Tween scaleTween;
    private Transform mainCameraTransform;

    private void Awake()
    {
        // 성능을 위해 메인 카메라의 Transform을 미리 캐싱합니다.
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    private void OnEnable()
    {
        // 1. 초기화: 스케일을 0으로 설정
        transform.localScale = Vector3.zero;

        // 2. 카메라 바라보기 (켜지는 순간 방향 고정)
        LookAtCamera();

        // 3. 기존 트윈 정지 및 재생
        scaleTween?.Kill();
        scaleTween = transform.DOScale(targetScale, 0.3f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    private void LateUpdate()
    {
        // 만약 카메라가 움직이거나 타워가 계속 회전한다면 
        // 매 프레임 카메라를 바라보도록 유지합니다.
        LookAtCamera();
    }

    private void LookAtCamera()
    {
        if (mainCameraTransform != null)
        {
            // 오브젝트의 정면이 카메라를 향하게 합니다.
            transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward,
                             mainCameraTransform.rotation * Vector3.up);
        }
    }
}