using UnityEngine;
using UnityEngine.EventSystems;

public class TowerHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 initPosition;
    private Camera mainCam;

    public LayerMask tileLayer;

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

                    Vector3 centerPos = TileManager.Instance.GetWorldPosition(interactor.x, interactor.y);

                    //centerPos.y = initPosition.y;

                    transform.position = centerPos;
                    initPosition = transform.position;
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
