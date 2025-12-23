using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;
using NUnit.Framework.Internal;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.WSA;

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
    //성공 토스트 메시지 
    [SerializeField] private GameObject succeedToastMessage; 
    //실패 토스트 메시지 
    [SerializeField] private GameObject failedToastMessage; 

    private GameObject fadeOutObject;
    private Coroutine fadeOutCoroutine;
    public float fadeOutDelay = 2;
    

    //클릭된 타워
    Tower currentTower;
    public Tower CurrentTower { get => currentTower; private set => currentTower = value; }

    //클릭된 타일 
    TileInteractor currentTile;
    public TileInteractor CurrentTile{ get => currentTile; private set => currentTile = value;}
    

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnChangedGold += UpdateGold;
        }
    }

    public void GachaBtn()
    {
       int towerid = TowerManager.Instance.Gacha();
        //Debug.Log(towerid);
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
        if(CurrentTile != null)
        {
            CurrentTile.transform.GetChild(3).gameObject.SetActive(false);
            CurrentTile = null;            
        }

        tileTransitionPanel.SetActive(false);
    }

    public void ConfirmTileTransition()
    {   
        if(CurrentTile != null)
        {
            //유효성 검사를 위한 임시 변경
            currentTile.ChangeTileType();

            bool isGenerated = PathNodeManager.Instance.GeneratePath();
            
            if (!isGenerated)
            {
                currentTile.ChangeTileType();
            }
            //Toast메시지 출력 
            OpenToastMessage(isGenerated);
            CloseTileTransitionPanel();    
        }
        else 
            return;
    }

    /// <summary>
    /// 성공 여부를 받아 해당하는 토스트 메시지를 출력 
    /// </summary>
    public void OpenToastMessage(bool isSuccess)
    {
        if(fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
            fadeOutObject.SetActive(false);
        }

        if(isSuccess)
        {
            succeedToastMessage.SetActive(true);
            fadeOutObject = succeedToastMessage;
            fadeOutCoroutine = StartCoroutine(FadeOutCanvasGroup(fadeOutObject, 0.5f, fadeOutDelay));
            
        }
        else
        {
            failedToastMessage.SetActive(true); 
            fadeOutObject = failedToastMessage;   
            fadeOutCoroutine = StartCoroutine(FadeOutCanvasGroup(failedToastMessage, 0.5f,  fadeOutDelay));
        }  
    }

    //타일 전환 창
    public void OpenTileTransitionPanel(TileInteractor SelectTile)
    {
        //하이라이트 끄기 
        if(CurrentTile != null)
        {
            CurrentTile.transform.GetChild(3).gameObject.SetActive(false);
            CurrentTile = null;   
        }
        CurrentTile = SelectTile;

        if(CurrentTile.Type == TileInfo.TYPE.Road )
        {
            tileTransitionPanel.SetActive(true);
            currentTileText.text = $"{SelectTile.Type}";
            toChangeTileText.text = $"Road";
            //todo 재화 상태에 따라 버튼이 달라지는 기능 
            
        }
        else if(CurrentTile.Type == TileInfo.TYPE.Wall)
        {
            tileTransitionPanel.SetActive(true);
            currentTileText.text = $"{SelectTile.Type}";
            toChangeTileText.text = $"Wall";
            //todo 재화 상태에 따라 버튼이 달라지는 기능 
        }
        else
        {
            return;
        }
    }

    public void SellTower()
    {
        if (CurrentTower != null)
        {
            TowerManager.Instance.SellTower(CurrentTower);
            CurrentTower = null;
        }
    }

    public void UpdateGold(int gold){
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

        if(tileTransitionPanel.activeSelf)
        {
            //패널 닫기 
            CloseTileTransitionPanel();
        }
        
    }

    private IEnumerator FadeOutCanvasGroup(GameObject fadeOutObject, float waitTime, float fadeOutTime)
    {   
        CanvasGroup canvasGroup;
        bool isSucceed= fadeOutObject.TryGetComponent<CanvasGroup>(out canvasGroup);
        
        WaitForSeconds wfs = new WaitForSeconds(waitTime);
        
        canvasGroup.alpha = 1;

        if(!isSucceed)
        {
            Debug.LogError("It has not have canvasGroup");
            yield break;
        }
        
        yield return wfs;

        float alpha = 1;
        float previous = 0;   

        while(alpha > 0)
        {
            
            previous += Time.deltaTime; 
            alpha = Mathf.Lerp(1, 0, previous/fadeOutTime);
            canvasGroup.alpha = alpha;
            
            yield return null;
        }

        
        fadeOutObject.SetActive(false);
        


    }

}
