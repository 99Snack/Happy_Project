using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AugmentItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI augmentName;
    [SerializeField] private TextMeshProUGUI description;

    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
    }

    public void Setup(AugmentData data)
    {
        //icon = Resources.Load<Image>($"Icon/{data.Icon_Resource}");

        string name = DataManager.Instance.LocalizationData[data.Name_STR].Ko;
        string desc = DataManager.Instance.LocalizationData[data.Desc_STR].Ko;

        object[] values = { data.Value_N, data.Value_M };

        desc = string.Format(desc, values);

        augmentName.text = name;
        description.text = desc;

        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => UIManager.Instance.OnClickAugment(data));
        }
    }
}
