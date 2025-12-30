using Mono.Cecil.Cil;
using System.Collections.Generic;
using UnityEngine;

public class ActivatedAugmentPanel : MonoBehaviour
{
    [SerializeField] private GameObject activateAugments;
    [SerializeField] private GameObject activateAugmentItem;
    private bool isVisible = false;

    private List<AugmentData> items = new List<AugmentData>();
    private List<GameObject> activateAugmentObject = new List<GameObject>();

    public void UpdateActivatedAugment()
    {
        var augments = AugmentManager.Instance.activeAugments;

        foreach (var augment in augments)
        {
            if (items.Contains(augment)) return;

            GameObject newObj = Instantiate(activateAugmentItem, activateAugments.transform);
            activateAugmentObject.Add(newObj);
            ActivatedAugmentItem item = newObj.GetComponent<ActivatedAugmentItem>();
            
            item.Setup(augment);

            items.Add(augment);
        }
    }

    public void VisibleButton()
    {
        if (items.Count <= 0) return;

        UpdateActivatedAugment();
       
        isVisible = !isVisible;

        activateAugments.SetActive(isVisible);

    }

    public void ClearActiveAugment()
    {
        foreach(var obj in activateAugmentObject)
        {
            Destroy(obj);
        }
        items.Clear();
    }        

}
