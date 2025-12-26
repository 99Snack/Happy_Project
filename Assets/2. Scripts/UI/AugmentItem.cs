using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        UIManager.Instance.OnClickAugment(data);
    }
}
