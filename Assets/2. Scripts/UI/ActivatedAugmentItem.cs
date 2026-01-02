using UnityEngine;
using UnityEngine.UI;

public class ActivatedAugmentItem : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Outline outline;

    public void Setup(AugmentData augment)
    {
        //image = Resources.Load($"Image/{augment.Icon_Resource}");

        Color color = augment.Tier switch
        {
            1 => Color.gray,
            2 => Color.orange,
            3 => Color.skyBlue,
            _ => Color.gray
        };

        outline.effectColor = color;
    }
}
