using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private Slider slider;

    public void Setup(WaveData wave)
    {
        //임의 스테이지 데이터 넣음
        //StageData stage = GameManager.Instance.StageInfo;
        StageData stage = DataManager.Instance.StageData[10001];
        var localStr = DataManager.Instance.LocalizationData;
        stageText.text = localStr[stage.StageName].Ko;

        //todo : 웨이브 총 수 리터럴로 넣음
        waveText.text = $"{localStr[wave.WaveName].Ko} /15";
    }

    public void UpdateSlider(int currentSpawnCount, int maxCount)
    {
        float current = (float)currentSpawnCount / maxCount;
        slider.DOValue(current, 2f).SetEase(Ease.Linear);
    }
}
