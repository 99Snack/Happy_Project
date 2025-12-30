using UnityEngine;

public class WavePreparation : MonoBehaviour
{
    public void GachaButton()
    {
        //타워 뽑기 비용 100
        if (GameManager.Instance.Gold >= TowerManager.GACHA_PRICE)
        {
            GameManager.Instance.Gold -= TowerManager.GACHA_PRICE;
            int towerid = TowerManager.Instance.Gacha();
            //Debug.Log(towerid);
        }
        else
        {
            //todo : 실패 사운드
            //SoundManager.Instance.PlaySFX(ClipName.Fail_sound);
        }
    }

    public void StartButton(){
        SpawnManager.Instance.OnStartButtonClick();
    }
}
