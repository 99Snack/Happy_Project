using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class TowerHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 initPosition;
    private Camera mainCam;

    public LayerMask tileLayer;
    private Tower tower;
    private TileInteractor originTile;
    private TileInteractor previousTile;
    private int towerRange; 

    private List<Vector2Int> currentHighLight;
    private List<Vector2Int> previousHighLight;



    private bool isBuild;

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
        towerRange = tower.Data.Range;
        currentHighLight = new List<Vector2Int>();
        previousHighLight = new List<Vector2Int>();
    }

    //드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        Ray ray = new Ray(transform.position + transform.up, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 2f, tileLayer))
        {
            originTile = hit.collider.GetComponent<TileInteractor>();
            
            //건설된 타워인지 확인 
            if (originTile != null) 
            {
                if(originTile.Type == TileInfo.TYPE.Wall)
                {

                    isBuild = true;
                }   
                else
                {
                    isBuild = false;
                    originTile.isAlreadyTower = false;
                }
                    
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (mainCam == null) return;
        
        if(isBuild) return;
        
        Ray ray = mainCam.ScreenPointToRay(eventData.position);
        
       
    
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, tileLayer))
        {
            Vector3 colPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            
            TileInteractor interactor = hit.collider.GetComponent<TileInteractor>();
            
            transform.position = colPos;

            if(interactor != null)
            {
                if(interactor.Type == TileInfo.TYPE.Wall || interactor.Type == TileInfo.TYPE.Road)
                {
                    if(currentHighLight.Count > 0)
                    {      
                        List<Vector2Int> temp = previousHighLight;
                        previousHighLight = currentHighLight;
                        currentHighLight  = temp;
                    }
                    currentHighLight.Clear();
                    GetRangeTile(new Vector2Int(interactor.X,interactor.Y),currentHighLight);

                    foreach(var coor in currentHighLight)
                    {
                        //현재에는 있지만 이전에는 없었던 타일 켜기 
                        if(!previousHighLight.Contains(coor))
                        {
                            TileInteractor coortile = TileManager.Instance.map.tiles[(coor.x,coor.y)];
                            UIManager.Instance.TurnOnHighlightTile(coortile,coortile.isAlreadyTower);
                        }
                    }

                    foreach(var coor in previousHighLight)
                    {
                        if(!currentHighLight.Contains(coor))
                        {
                            //이전에는 있었지만 현재에는 없는 타일 끄기 
                            TileInteractor coortile = TileManager.Instance.map.tiles[(coor.x,coor.y)];
                            UIManager.Instance.TurnOffHighlightTile(coortile);
                        }
                    }
                }
            } 
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if(isBuild) return;
        Ray ray = mainCam.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, tileLayer))
        {
            TileInteractor interactor = hit.collider.GetComponent<TileInteractor>();
            if(currentHighLight.Count > 0)
            {
                foreach(var coor in currentHighLight)
                {
                    TileInteractor coortile = TileManager.Instance.map.tiles[(coor.x,coor.y)];
                    UIManager.Instance.TurnOffHighlightTile(coortile);
                }    
            }
            
            if (interactor != null && !interactor.isAlreadyTower)
            {
                if (interactor.Type == TileInfo.TYPE.Wall || interactor.Type == TileInfo.TYPE.Wait)
                {
                    //타일 중앙 탑에 위치
                    Vector3 centerPos = TileManager.Instance.GetWorldPosition(interactor.X, interactor.Y);
                    centerPos.y = hit.point.y;
                    transform.position = centerPos;

                    //타워 이펙트
                    ObjectPoolManager.Instance.SpawnFromPool("attacheffect", centerPos, Quaternion.identity);

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

                    //증강 재적용
                    tower.UpdateConditionAugment();

                    //드래그 드랍했을때 타워정보창 닫기
                    UIManager.Instance.CloseTowerInfo();

                    //todo : 타워 배치 성공 시 사운드
                    SoundManager.Instance.PlaySFX(ClipName.Success_sound);

                    return;
                }
                else
                {
                    Debug.LogError($"현재 타일의 타입은 {interactor.Type}입니다");
                }
            }
            else if(interactor.isAlreadyTower && interactor.Type == TileInfo.TYPE.Wall)
            {
                CameraManager.Instance.ShakeCam();
                UIManager.Instance.OpenAttachToastMessage();
            }
        }

        transform.position = initPosition;

        if (originTile != null)
        {
            originTile.isAlreadyTower = true;
        }
    }

    void GetRangeTile(Vector2Int center, List<Vector2Int> inRangeTIles)
    {
        for(int x = -towerRange; x <=  towerRange; x++ )
        {
            for(int y = -towerRange; y <= towerRange; y++ )
            {
                Vector2Int temp = new Vector2Int(center.x + x, center.y + y);
                
                if(!TileManager.Instance.IsValidCoordinate(temp.x,temp.y))
                {
                    continue;
                }
                
                TileInteractor coortile = TileManager.Instance.map.tiles[(temp.x,temp.y)];
                
                if(coortile.Type == TileInfo.TYPE.Wait || coortile.Type == TileInfo.TYPE.EnemyBase || coortile.Type== TileInfo.TYPE.AllyBase)
                {
                    continue;
                }
                inRangeTIles.Add(temp);
            }
        }    
    }

}
