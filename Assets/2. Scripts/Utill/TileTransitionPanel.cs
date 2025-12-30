using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileTransitionPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI priceText;

    [SerializeField] private TextMeshProUGUI currentTileText;
    [SerializeField] private TextMeshProUGUI toChangeTileText;

    [SerializeField] private CanvasGroup succeedToast;
    [SerializeField] private CanvasGroup failedToast;

    [SerializeField] private Button confirmButton;

    TileInteractor currentTile;

    public void Setup(TileInteractor selectTile)
    {
        //하이라이트 끄기 
        if (currentTile != null)
        {
            currentTile.transform.GetChild(3).gameObject.SetActive(false);
            currentTile = null;
        }
        currentTile = selectTile;

        if (currentTile.Type != TileInfo.TYPE.Wall && currentTile.Type != TileInfo.TYPE.Road) return;

        currentTileText.text = $"{selectTile.Type}";
        toChangeTileText.text = selectTile.Type == TileInfo.TYPE.Road ? "Wall" : "Road";
        priceText.text = $"{TileManager.TRANSITION_PRICE}";

        //todo 재화 상태에 따라 버튼이 달라지는 기능
        if (GameManager.Instance.Gold >= TileManager.TRANSITION_PRICE)
        {
            confirmButton.interactable = true;
        }
        else
        {
            confirmButton.interactable = false;
        }
    }

    /// <summary>
    /// true : succeed, false : failed
    /// </summary>
    public void OpenTileToastMessage(bool isSuccess)
    {
        var targetToast = isSuccess == true ? succeedToast : failedToast;
        var otherToast = isSuccess == false ? succeedToast : failedToast;

        targetToast.alpha = 1f;

        targetToast.gameObject.SetActive(true);
        otherToast.gameObject.SetActive(false);


        targetToast.DOFade(0f, 1.25f).SetEase(Ease.Linear);
    }

    public void Cancel()
    {
        //하이라이트 제거 
        if (currentTile != null)
        {
            currentTile.transform.GetChild(3).gameObject.SetActive(false);
            currentTile = null;
        }
        gameObject.SetActive(false);
    }

    public void Confirm()
    {
        if (currentTile != null)
        {

            //유효성 검사를 위한 임시 변경
            currentTile.ChangeTileType();

            bool isGenerated = PathNodeManager.Instance.GeneratePath();

            if (!isGenerated)
            {
                currentTile.ChangeTileType();
                //todo : 실패 피드백 사운드
                SoundManager.Instance.PlaySFX(ClipName.Fail_sound);
            }
            else
            {
                GameManager.Instance.Gold -= TileManager.TRANSITION_PRICE;
                //todo : 성공 피드백 사운드
                SoundManager.Instance.PlaySFX(ClipName.Success_sound);
            }

            //Toast메시지 출력 
            OpenTileToastMessage(isGenerated);

            gameObject.SetActive(false);
        }
        else
            return;
    }


}
