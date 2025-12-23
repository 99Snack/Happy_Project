using System.Collections;
using TMPro;
using UnityEngine;

public class TileUI : MonoBehaviour
{
    [SerializeField] private GameObject changeTilePanel; //타일 변환 패널 
    [SerializeField] private TextMeshProUGUI currentTileText; // 현재 타일 이름이 출력될 텍스트
    [SerializeField] private TextMeshProUGUI toChangeTileText; // 바뀔 타일 이름이 출력될 텍스트

    [SerializeField] private GameObject succeedToastMessage; //성공 시 출력되는 토스트 메시지 
    [SerializeField] private GameObject failedToastMessage; //실패 시 출력되는 토스트 메시지 
    private TileData currentTile; //현재 타일 데이터 

    public int tileCost; //현재 타일 비용
    public int userMoney; //User의 현재 재산

    [HideInInspector]
    public bool isChanged = false;

    /// <summary>
    /// 해당 타일이 변경 될 수 있는 타일이면 열기  
    /// </summary>
    /// <param name="SelectTileData">선택된 타일의 데이터</param>
    public void OpenChangeTilePanel(TileData SelectTileData)
    {
        currentTile = SelectTileData;

        if(currentTile.Type == TileData.TYPE.Road )
        {
            changeTilePanel.SetActive(true);
            currentTileText.text = $"{SelectTileData.Type}";
            toChangeTileText.text = $"Wall"; //(테스트용)
        }
        else if(currentTile.Type == TileData.TYPE.Wall)
        {
            changeTilePanel.SetActive(true);
            currentTileText.text = $"{SelectTileData.Type}";
            toChangeTileText.text = $"Wall";
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// 타일 변경 패널 닫기  
    /// </summary>
    public void CloseChangeTilePanel()
	{
		changeTilePanel.SetActive(false);
	}

    /// <summary>
    /// 취소 버튼 입력 시 패널 없애기 
    /// </summary>
    public void CancelChangeTile()
    {
       changeTilePanel.SetActive(false);
    }


    /// <summary>
    /// 확인 버튼을 눌렀을 때 유효성 검사 후 재화를 확인하고 
    /// 타일을 변경 되었을 시 true를 반환
    /// </summary>
    public void ConfirmChangeTile()
    {
        isChanged = false;

        //유효성 검사를 위한 임시 변경 
        ChangeCurrentTileType();

        bool isGenerated = PathNodeManager.Instance.GeneratePath();
    
        //임시 변경했던 타입 복귀
        ChangeCurrentTileType();
        OpenToastMessage(isGenerated);
        isChanged = true;
        CloseChangeTilePanel();
    
    }

    /// <summary>
    /// 성공 여부를 받아 해당하는 토스트 메시지를 출력 
    /// </summary>
    public void OpenToastMessage(bool isSuccess)
    {
        if(isSuccess)
        {
            succeedToastMessage.SetActive(true);
            FadeOut(succeedToastMessage, 2);
            
        }
        else
        {
            failedToastMessage.SetActive(true);
            FadeOut(failedToastMessage, 2);
            
        }
    }
    /// <summary>
    /// 현재 타일의 타입을 변경하는 메서드 
    /// </summary>
    private void ChangeCurrentTileType()
    {
        if (currentTile.Type == TileData.TYPE.Wall)
        {
            currentTile.Type = TileData.TYPE.Road;
        }
        else
        {
            currentTile.Type = TileData.TYPE.Wall;
        }
    }


    //todo: 페이드 아웃 
    private IEnumerator FadeOut (GameObject fadeOutObject, float time)
    {
        WaitForSeconds wfs = new WaitForSeconds(time);

        //todo: 해당 물체의 알파값을 서서히 줄이다가 일정 값이 되면 사라짐 
        yield return wfs;

        fadeOutObject.SetActive(false);
    }




}
