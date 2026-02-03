using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;

// https://stackoverflow.com/questions/30766020/how-to-scroll-to-a-specific-element-in-scrollrect-with-unity-ui

public class ScrollSnapTo : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform contentPanel;

    public RectTransform contentHeight;

    public int paddingTop = 10;
    public int paddingBottom = 10;

    //public void SnapTo(RectTransform target)
    //{
    //    Canvas.ForceUpdateCanvases();

    //    contentPanel.anchoredPosition =
    //            (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)
    //            - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
    //}

    public void SnapTo(RectTransform obj)//, RectTransform contentHeight)
    {
        var objPosition = (Vector2)scrollRect.transform.InverseTransformPoint(obj.position);
        //var scrollHeight = scrollRect.GetComponent<RectTransform>().rect.height;
        var scrollHeight = contentHeight.rect.height;
        var objHeight = obj.rect.height;

        if (objPosition.y > scrollHeight / 2)
        {
            contentPanel.localPosition = new Vector2(contentPanel.localPosition.x,
                contentPanel.localPosition.y - objHeight - paddingTop);
        }

        if (objPosition.y < -scrollHeight / 2)
        {
            contentPanel.localPosition = new Vector2(contentPanel.localPosition.x,
        contentPanel.localPosition.y + objHeight + paddingBottom);
        }
    }

}
