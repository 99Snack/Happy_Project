using TMPro;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Android;
using UnityEngine.Tilemaps;


public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public static UIManager Instance { get => instance; private set => instance = value; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }

    [SerializeField] private TextMeshProUGUI goldText;
    //타워 정보창
    [SerializeField] private GameObject towerInfoPanel;
    [SerializeField] private TextMeshProUGUI towerNameText;
    //타일 전환 창 
    [SerializeField] private GameObject tileTransitionPanel;
    [SerializeField] private TextMeshProUGUI currentTileText;
    [SerializeField] private TextMeshProUGUI toChangeTileText;
    //타일 변환 성공 토스트 메시지 
    [SerializeField] private GameObject transitionSucceedToast;
    //타일 변환 실패 토스트 메시지 
    [SerializeField] private GameObject transitionFailedToast;
    //타워 배치 실패 메시지 
    [SerializeField] private GameObject attachToastMessage;

    //페이드 아웃 관련 변수
    private GameObject fadeOutObject;
    private Coroutine fadeOutCoroutine;
    private const float fadeOutDelay = 0.75f;


    //클릭된 타워
    Tower currentTower;
    public Tower CurrentTower { get => currentTower; private set => currentTower = value; }

    //클릭된 타일 
    TileInteractor currentTile;
    public TileInteractor CurrentTile { get => currentTile; private set => currentTile = value; }


    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnChangedGold += UpdateGold;
        }
    }

    public void GachaBtn()
    {
        //타워 뽑기 비용 100
        if (GameManager.Instance.Gold >= TowerManager.GACHA_PRICE)
        {
            GameManager.Instance.Gold -= TowerManager.GACHA_PRICE;
            int towerid = TowerManager.Instance.Gacha();
            //Debug.Log(towerid);
        }
    }

    public void OpenTowerInfo(Tower SelectTower)
    {
        CurrentTower = SelectTower;

        //todo : 해당 타워 정보로 갱신
        towerInfoPanel.SetActive(true);
        towerNameText.text = $"{SelectTower.Data.TowerID}";

    }

    public void CloseTowerInfo()
    {
        if (CurrentTower != null)
        {
            CurrentTower = null;
        }

        towerInfoPanel.SetActive(false);
    }

    public void CloseTileTransitionPanel()
    {
        //하이라이트 제거 
        if (CurrentTile != null)
        {
            CurrentTile.transform.GetChild(3).gameObject.SetActive(false);
            CurrentTile = null;
        }

        tileTransitionPanel.SetActive(false);
    }

    public void ConfirmTileTransition()
    {
        if (CurrentTile != null)
        {
            //유효성 검사를 위한 임시 변경
            currentTile.ChangeTileType();

            bool isGenerated = PathNodeManager.Instance.GeneratePath();

            if (!isGenerated)
            {
                currentTile.ChangeTileType();
            }
            //Toast메시지 출력 
            OpenTileToastMessage(isGenerated);
            CloseTileTransitionPanel();
        }
        else
            return;
    }

    /// <summary>
    /// 타일 변환 성공 여부를 받아 해당하는 토스트 메시지를 출력 
    /// </summary>
    public void OpenTileToastMessage(bool isSuccess)
    {
        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
            fadeOutObject.SetActive(false);
        }

        if (isSuccess)
        {
            transitionSucceedToast.SetActive(true);
            fadeOutObject = transitionSucceedToast;
            fadeOutCoroutine = StartCoroutine(FadeOutCanvasGroup(fadeOutObject, 0.5f, fadeOutDelay));

        }
        else
        {
            transitionFailedToast.SetActive(true);
            fadeOutObject = transitionFailedToast;
            fadeOutCoroutine = StartCoroutine(FadeOutCanvasGroup(transitionFailedToast, 0.5f, fadeOutDelay));
        }
    }
    public void OpenAttachToastMessage()
    {
        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
            fadeOutObject.SetActive(false);
        }

        attachToastMessage.SetActive(true);
        fadeOutObject = attachToastMessage;
        fadeOutCoroutine = StartCoroutine(FadeOutCanvasGroup(attachToastMessage, 0.5f, fadeOutDelay));
    }

    //타일 전환 창
    public void OpenTileTransitionPanel(TileInteractor SelectTile)
    {
        //하이라이트 끄기 
        if (CurrentTile != null)
        {
            CurrentTile.transform.GetChild(3).gameObject.SetActive(false);
            CurrentTile = null;
        }
        CurrentTile = SelectTile;

        if (currentTile.Type != TileInfo.TYPE.Wall && currentTile.Type != TileInfo.TYPE.Road) return;

        tileTransitionPanel.SetActive(true);
        currentTileText.text = $"{SelectTile.Type}";
        toChangeTileText.text = SelectTile.Type == TileInfo.TYPE.Road ? "Wall" : "Road";

        //todo 재화 상태에 따라 버튼이 달라지는 기능
        //if (GameManager.Instance.Gold >= TileManager.TRANSITION_PRICE)
        //{
        //    //체크 표시 활성화
        //}
        //else
        //{
        //    //체크 표시 비활성화
        //}

        //if (CurrentTile.Type == TileInfo.TYPE.Road )
        //{
        //    tileTransitionPanel.SetActive(true);
        //    currentTileText.text = $"{SelectTile.Type}";
        //    toChangeTileText.text = $"Wall";

        //}
        //else if(CurrentTile.Type == TileInfo.TYPE.Wall)
        //{
        //    tileTransitionPanel.SetActive(true);
        //    currentTileText.text = $"{SelectTile.Type}";
        //    toChangeTileText.text = $"Road";
        //}
    }

    public void TurnOnHighlightTile(TileInteractor tileInteractor, bool isValid)
    {
        int num = isValid ? 6:5;

        // 하이라이트
        tileInteractor.gameObject.transform.GetChild(num).gameObject.SetActive(true);
    }

    public void TurnOffHighlightTile(TileInteractor tileInteractor)
    {
        // 모든 하이라이트 비활성화  
        tileInteractor.gameObject.transform.GetChild(5).gameObject.SetActive(false);
        tileInteractor.gameObject.transform.GetChild(6).gameObject.SetActive(false);
    }

    public void SellTower()
    {
        if (CurrentTower != null)
        {
            TowerManager.Instance.SellTower(CurrentTower);
            CurrentTower = null;
        }
    }

    public void UpdateGold(int gold)
    {
        goldText.text = $"Gold : {gold}";
    }

    private void Update()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            HandleGlobalInput();
        }
    }

    private void HandleGlobalInput()
    {
        //UI 영역을 클릭했다면
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        //타워 클릭 여부 확인
        Vector2 pointerPosition = Pointer.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(pointerPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Tower"))
            {
                //OpenTowerInfo();
                return;
            }
        }

        if (towerInfoPanel.activeSelf)
        {
            //패널 닫기
            CloseTowerInfo();
        }

        if (tileTransitionPanel.activeSelf)
        {
            //패널 닫기 
            CloseTileTransitionPanel();
        }

    }

    private IEnumerator FadeOutCanvasGroup(GameObject fadeOutObject, float waitTime, float fadeOutTime)
    {
        CanvasGroup canvasGroup;
        bool isSucceed = fadeOutObject.TryGetComponent<CanvasGroup>(out canvasGroup);

        WaitForSeconds wfs = new WaitForSeconds(waitTime);

        canvasGroup.alpha = 1;

        if (!isSucceed)
        {
            Debug.LogError("It has not have canvasGroup");
            yield break;
        }

        yield return wfs;

        float alpha = 1;
        float previous = 0; //지난 시간   

        while (alpha > 0)
        {

            previous += Time.deltaTime;
            alpha = Mathf.Lerp(1, 0, previous / fadeOutTime);
            canvasGroup.alpha = alpha;

            yield return null;
        }


        fadeOutObject.SetActive(false);

    }

}
