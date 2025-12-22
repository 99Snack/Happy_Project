using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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

    //클릭된 타워
    Tower currentTower;
    public Tower CurrentTower { get => currentTower; private set => currentTower = value; }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnChangedGold += UpdateGold;
        }
    }

    public void GachaBtn()
    {
        Debug.Log(TowerManager.Instance.Gacha());
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


    //타일 전환 창
    public void TileTransitionPanel()
    {

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
    }
}
