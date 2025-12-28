using System.Collections.Generic;
using UnityEngine;

public class ActivatedAugmentPanel : MonoBehaviour
{
    [SerializeField] private GameObject activateAugments;
    [SerializeField] private GameObject activateAugmentItem;
    private bool isVisible = false;

    private List<AugmentData> items = new List<AugmentData>();

    public void UpdateActivatedAugment()
    {
        var augments = AugmentManager.Instance.activeAugments;
        foreach (var augment in augments)
        {
            if (items.Contains(augment)) return;

            GameObject newObj = Instantiate(activateAugmentItem, activateAugments.transform);
            ActivatedAugmentItem item = newObj.GetComponent<ActivatedAugmentItem>();
            item.Setup(augment);

            items.Add(augment);
        }
    }

    public void VisibleButton()
    {
        UpdateActivatedAugment();

        isVisible = !isVisible;

        activateAugments.SetActive(isVisible);
    }
}
