using DG.Tweening;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

        DontDestroyOnLoad(gameObject);
    }

    public Action OnUIInitialized;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        GameManager.Instance.OnChangedGold += UpdateGold;
    }

    //현재 생성된 UI 루트 오브젝트를 저장할 변수
    [SerializeField] private GameObject currentSceneUI;
    public Transform stageTrans;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //기존 씬의 UI가 남아있다면 삭제
        if (currentSceneUI != null)
        {
            Destroy(currentSceneUI);
        }

        //새 씬 이름에 맞는 UI 프리팹 생성
        currentSceneUI = OpenSceneUI($"{scene.name}UI");

        //SetupInGameReferences(currentSceneUI);
    }

    private GameObject OpenSceneUI(string sceneName)
    {
        GameObject sceneUI = null;
        GameObject prefab = Resources.Load<GameObject>($"Prefab/UI/{sceneName}");

        //Debug.Log($"Prefab/UI/{sceneName}");
        if (prefab != null)
        {
            sceneUI = Instantiate(prefab, transform);

            if (sceneName.Equals("InGameUI"))
            {
                SetupInGameReferences(sceneUI);
                SoundManager.Instance.PlayBGM(ClipName.Ingame_bgm);
            }
            else if (sceneName.Equals("LobbyUI"))
            {
                stageTrans = sceneUI.GetComponent<LobbyUiContainer>().stageTrans;
                OnUIInitialized?.Invoke();
                SoundManager.Instance.PlayBGM(ClipName.Main_bgm);
            }
        }


        return sceneUI;
    }

    private void SetupInGameReferences(GameObject root)
    {
        var container = root.GetComponent<InGameUIContainer>();
        if (container != null)
        {
            this.stageExitPanel = container.stageBackButton;
            this.wavePreparation = container.WavePreparation;
            this.goldText = container.GoldText;
            this.attachTowerToast = container.attachTowerFaildToast;
            this.gachaFailedToast = container.gachaFaildToast;
            this.augmentPanel = container.AugmentPanel;
            this.activatedAugmentPanel = container.ActivatedAugmentPanel;
            this.waveResultPanel = container.WaveResultPanel;
            this.stageResultPanel = container.StageResultPanel;
            this.allyBaseCampPanel = container.AllyBaseCampPanel;
            this.towerInfoPanel = container.TowerInfoPanel;
            this.tileTransitionPanel = container.TileTransitionPanel;
            this.stageInfoPanel = container.StageInfoPanel;
        }
    }

    public void GoToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void GoToStage()
    {
        SceneManager.LoadScene("Stage");
    }

    public void GoToInGame()
    {
        SceneManager.LoadScene("InGame");
    }

    //위에 3개 메서드 아래 메서드로 통일
    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터 Play 종료
#else
        Application.Quit(); // 빌드 게임 종료
#endif
    }

    //로비씬 Exit 팝업
    public GameObject LobbyExitPopup;
    public void ShowLobbyExitPopup()
    {
        LobbyExitPopup.SetActive(true);
    }

    public void CloseLobbyExitPopup()
    {
        LobbyExitPopup.SetActive(false);
    }

    //인게임 -> 로비씬 팝업
    [SerializeField] private GameObject stageExitPanel;
    public void OpenStageExitPanel() => stageExitPanel.SetActive(true);
    public void CloseStageExitPanel() => stageExitPanel.SetActive(false);

    [SerializeField] private TextMeshProUGUI goldText;

    //타워 배치 실패 메시지 
    [SerializeField] private AugmentPanel augmentPanel;
    [SerializeField] private ActivatedAugmentPanel activatedAugmentPanel;

    //웨이브 결과 창
    [SerializeField] private WaveResultPanel waveResultPanel;
    public bool IsActiveWaveResultPanel() => waveResultPanel.gameObject.activeSelf;
    public void OpenWaveResultPanel(int result)
    {
        waveResultPanel.gameObject.SetActive(true);
        waveResultPanel.OnResultAnimation(result);
    }
    public void CloseWaveResultPanel() => waveResultPanel.gameObject.SetActive(false);

    //스테이지 결과 창
    [SerializeField] private StageResultPanel stageResultPanel;
    public bool IsActiveStageResultPanel() => stageResultPanel.gameObject.activeSelf;
    public void OpenStageResultPanel(int result)
    {
        stageResultPanel.Setup(result);

        stageResultPanel.gameObject.SetActive(true);
    }

    //정비 시간동안 떠있어야할 패널
    [SerializeField] private GameObject wavePreparation;
    public void OpenWavePreparationPanel() => wavePreparation.SetActive(true);
    public void CloseWavePreparationPanel() => wavePreparation.SetActive(false);

    //베이스 캠프 패널 관련
    [SerializeField] private AllyBaseCampPanel allyBaseCampPanel;
    public void OpenAllyBaseCampPanel() => allyBaseCampPanel.gameObject.SetActive(true);
    public void UpdateAllyBaseCampHp() => allyBaseCampPanel.UpdateAllyBaseCampHp();
    public void CloseAllyBaseCampPanel() => allyBaseCampPanel.gameObject.SetActive(false);

    //증강 패널 관련    
    public void ClearActivatedAugmentPanel() => activatedAugmentPanel.ClearActiveAugment();

    //페이드 아웃 관련 변수
    private GameObject fadeOutObject;
    private Coroutine fadeOutCoroutine;
    private const float fadeOutDelay = 0.75f;

    //클릭된 타워
    Tower currentTower;
    public Tower CurrentTower { get => currentTower; private set => currentTower = value; }
    //타워 정보창
    [SerializeField] private TowerInfoPanel towerInfoPanel;
    public void OpenTowerInfoPanel(Tower selectTower)
    {
        currentTower = selectTower;

        towerInfoPanel.Setup(currentTower);

        towerInfoPanel.gameObject.SetActive(true);
    }

    public void CloseTowerInfo()
    {
        if (CurrentTower != null)
        {
            CurrentTower = null;
        }

        towerInfoPanel.gameObject.SetActive(false);
    }


    //클릭된 타일 
    TileInteractor currentTile;
    public TileInteractor CurrentTile { get => currentTile; private set => currentTile = value; }

    public void OpenAugmentPanel(int wave)
    {
        //현재 스테이지의 정보르 토대로 증강 가져오기
        var augments = AugmentManager.Instance.GetGeneratorRandomAugment(wave);
        augmentPanel.Setup(augments);

        augmentPanel.gameObject.SetActive(true);
    }
    public void CloseAugmentPanel() => augmentPanel.gameObject.SetActive(false);

    [SerializeField] private CanvasGroup attachTowerToast;
    public void OpenAttachToastMessage()
    {
        attachTowerToast.DOKill();

        attachTowerToast.alpha = 1f;
        attachTowerToast.gameObject.SetActive(true);

        attachTowerToast.DOFade(0f, 1.25f).SetEase(Ease.Linear).OnComplete(() => attachTowerToast.gameObject.SetActive(false));
    }

    [SerializeField] private CanvasGroup gachaFailedToast;
    public void OpenGachaFailedToast()
    {
        gachaFailedToast.DOKill();

        gachaFailedToast.alpha = 1f;
        gachaFailedToast.gameObject.SetActive(true);

        gachaFailedToast.DOFade(0f, 1.25f).SetEase(Ease.Linear).OnComplete(() => gachaFailedToast.gameObject.SetActive(false));
    }

    //타일 전환 창
    [SerializeField] private TileTransitionPanel tileTransitionPanel;
    public void OpenTileTransitionPanel(TileInteractor SelectTile)
    {
        tileTransitionPanel.Setup(SelectTile);

        tileTransitionPanel.gameObject.SetActive(true);
    }


    public void TurnOnHighlightTile(TileInteractor tileInteractor, bool isValid)
    {
        int num = isValid ? 6 : 5;

        // 하이라이트
        tileInteractor.gameObject.transform.GetChild(num).gameObject.SetActive(true);
    }

    public void TurnOffHighlightTile(TileInteractor tileInteractor)
    {
        // 모든 하이라이트 비활성화  
        tileInteractor.gameObject.transform.GetChild(5).gameObject.SetActive(false);
        tileInteractor.gameObject.transform.GetChild(6).gameObject.SetActive(false);
    }

    public void UpdateGold(int gold)
    {
        if (goldText != null)
        {
            goldText.text = $"Gold : {gold}";
        }
    }


    private void Update()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            HandleGlobalInput();
        }

        //  if (Keyboard.current != null &&
        //  Keyboard.current.f10Key.wasPressedThisFrame)
        //  {
        //      OpenAugmentPanel(1);
        //  }

        //  if (Keyboard.current != null &&
        // Keyboard.current.f11Key.wasPressedThisFrame)
        //  {
        //      OpenAugmentPanel(4);
        //  }

        //  if (Keyboard.current != null &&
        //Keyboard.current.f12Key.wasPressedThisFrame)
        //  {
        //      OpenAugmentPanel(9);
        //  }
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

        if (towerInfoPanel != null && towerInfoPanel.gameObject.activeSelf)
        {
            //패널 닫기
            CloseTowerInfo();
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

    //스테이지 정보 창 관련
    [SerializeField] private StageInfoPanel stageInfoPanel;
    public void UpdateStageInfo(WaveData wave)
    {
        stageInfoPanel.Setup(wave);
    }

    public void UpdateWaveSlider(int current, int max)
    {
        //Debug.Log($"{current} : {max}");
        stageInfoPanel.UpdateSlider(current, max);
    }
}
