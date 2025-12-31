using UnityEngine;
using System.Collections.Generic;

public class ObjectCenterLayout : MonoBehaviour
{
    private float spacing = 0.25f;

    public void RefreshLayout()
    {
        List<Transform> activeStars = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.gameObject.activeSelf)
            {
                activeStars.Add(child);
            }
        }

        int count = activeStars.Count;
        if (count == 0) return;

        float totalWidth = (count - 1) * spacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            activeStars[i].localPosition = new Vector3(startX + (i * spacing), 0, 0);
        }
    }
}