using UnityEngine;
using UnityEngine.EventSystems;

public class TowerHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 initPosition;
    private Camera mainCam;

    public LayerMask tileLayer;
    private TowerTestCode tower;

    private void Awake()
    {
        mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("메인 카메라가 없습니다.");
        }
    }

    private void Start()
    {
        initPosition = transform.position;

        tower = GetComponent<TowerTestCode>();
    }

    //드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        //initPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (mainCam == null) return;

        Ray ray = mainCam.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, tileLayer))
        {
            Vector3 colPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            transform.position = colPos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Ray ray = mainCam.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, tileLayer))
        {
            TileInteractor interactor = hit.collider.GetComponent<TileInteractor>();

            if (interactor != null)
            {
                if (interactor.type == TileData.TYPE.Wall || interactor.type == TileData.TYPE.None)
                {
                    //타일 중앙 탑에 위치
                    Vector3 centerPos = TileManager.Instance.GetWorldPosition(interactor.x, interactor.y);
                    centerPos.y = hit.point.y;
                    transform.position = centerPos;

                    //타워 좌표 설정
                    tower.Setup(interactor.x, interactor.y);

                    //초기 위치 변경
                    initPosition = transform.position;

                    //부모 계층 변경
                    transform.SetParent(hit.transform);

                    return;
                }
                else{
                    Debug.LogError($"현재 타일의 타입은 {interactor.type}입니다");
                }
            }
        }

        transform.position = initPosition;

    }


}
