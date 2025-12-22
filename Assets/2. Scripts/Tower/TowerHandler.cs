using UnityEngine;
using UnityEngine.EventSystems;

public class TowerHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 initPosition;
    private Camera mainCam;

    public LayerMask tileLayer;
    private Tower tower;
    private TileInteractor originTile;

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

        tower = GetComponent<Tower>();
    }

    //드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        Ray ray = new Ray(transform.position + transform.up, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 2f, tileLayer))
        {
            originTile = hit.collider.GetComponent<TileInteractor>();
            if (originTile != null) originTile.isAlreadyTower = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (mainCam == null) return;

        Ray ray = mainCam.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, tileLayer))
        {
            Vector3 colPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            transform.position = colPos;

            //todo : 타워가 있는지 없는지 확인하는 로직
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Ray ray = mainCam.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, tileLayer))
        {
            TileInteractor interactor = hit.collider.GetComponent<TileInteractor>();

            if (interactor != null && !interactor.isAlreadyTower)
            {
                if (interactor.Type == TileData.TYPE.Wall || interactor.Type == TileData.TYPE.Wait)
                {
                    //todo: 타일에 배치하면 .Towerslot tile로 waitaus towerslot wait으로
                    if (interactor.Type == TileData.TYPE.Wall)
                    {
                        tower.TowerSlot = TowerSlot.Tile;
                    }
                    else if (interactor.Type == TileData.TYPE.Wait)
                    {
                        tower.TowerSlot = TowerSlot.Wait;
                    }

                    //타일 중앙 탑에 위치
                    Vector3 centerPos = TileManager.Instance.GetWorldPosition(interactor.X, interactor.Y);
                    centerPos.y = hit.point.y;
                    transform.position = centerPos;

                    //타워 좌표 설정
                    tower.SetCoord(interactor.X, interactor.Y);

                    //초기 위치 변경
                    initPosition = transform.position;

                    //부모 계층 변경
                    transform.SetParent(hit.transform);

                    //타일에 타워를 건설했다는 표시하기
                    interactor.isAlreadyTower = true;
                    originTile = interactor;

                    //타워의 현재 타일 변경
                    tower.SetMyTile(interactor);

                    //드래그 드랍했을때 타워정보창 닫기
                    UIManager.Instance.CloseTowerInfo();

                    return;
                }
                else
                {
                    Debug.LogError($"현재 타일의 타입은 {interactor.Type}입니다");
                }
            }
        }

        transform.position = initPosition;

        if (originTile != null)
        {
            originTile.isAlreadyTower = true;
        }
    }


}
