using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.HableCurve;

public class AugmentItem : MonoBehaviour
{
    private AugmentData data;

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI augmentName;
    [SerializeField] private TextMeshProUGUI description;


    public void Setup(AugmentData data)
    {
        this.data = data;
        //icon = Resources.Load<Image>($"Icon/{data.Icon_Resource}");

        string name = DataManager.Instance.LocalizationData[data.Name_STR].Ko;
        string desc = DataManager.Instance.LocalizationData[data.Desc_STR].Ko;

        object[] values = { data.Value_N, data.Value_M };

        desc = string.Format(desc, values);

        augmentName.text = name;
        description.text = desc;

    }

   public void SelectAugment(){
        AugmentManager.Instance.ActivateAugment(data);

        //경제 공용
        if (data.Category == 2 && data.Tag == 0)
        {
            //자원확보1,2,3
            GameManager.Instance.Gold += data.Value_N;
        }

        UIManager.Instance.CloseAugmentPanel();
    }
}
