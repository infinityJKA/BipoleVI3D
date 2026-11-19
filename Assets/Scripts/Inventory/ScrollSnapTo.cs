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
    [HideInInspector] public RectTransform viewport;


    public int paddingTop = 10;
    public int paddingBottom = 10;


    private RectTransform ViewRect => viewport != null ? viewport : scrollRect.GetComponent<RectTransform>();

    public void SnapTo(RectTransform target)
    {
        viewport = contentHeight;

        if (scrollRect == null || contentPanel == null || target == null)
            return;

        // Ensure layout groups / ContentSizeFitter have up-to-date positions before reading rects
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentPanel);

        RectTransform view = ViewRect;

        // Get target's 4 corners in world space, then convert into viewport local space
        Vector3[] targetCorners = new Vector3[4];
        target.GetWorldCorners(targetCorners);

        float minY = float.PositiveInfinity;
        float maxY = float.NegativeInfinity;

        for (int i = 0; i < 4; i++)
        {
            Vector3 localCorner = view.InverseTransformPoint(targetCorners[i]);
            if (localCorner.y < minY) minY = localCorner.y;
            if (localCorner.y > maxY) maxY = localCorner.y;
        }

        // Viewport Y bounds in viewport's own local space
        float viewTop = view.rect.yMax - paddingTop;
        float viewBottom = view.rect.yMin + paddingBottom;

        float deltaY = 0f;

        // Target extends above the visible top edge (e.g., wrapped from bottom to top)
        if (maxY > viewTop)
        {
            deltaY = maxY - viewTop;
        }
        // Target extends below the visible bottom edge (e.g., wrapped from top to bottom)
        else if (minY < viewBottom)
        {
            deltaY = minY - viewBottom;
        }

        if (Mathf.Approximately(deltaY, 0f))
            return;

        // Moving contentPanel DOWN (-deltaY) brings an item that is ABOVE the viewport DOWN into view
        Vector3 contentLocalPos = contentPanel.localPosition;
        contentLocalPos.y -= deltaY;
        contentPanel.localPosition = contentLocalPos;

        // Stop any active inertia/momentum so it doesn't fight the snap
        scrollRect.velocity = Vector2.zero;
    }


    ////public void SnapTo(RectTransform target)
    ////{
    ////    Canvas.ForceUpdateCanvases();

    ////    contentPanel.anchoredPosition =
    ////            (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)
    ////            - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
    ////}

    //public void SnapTo(RectTransform obj)//, RectTransform contentHeight)
    //{
    //    var objPosition = (Vector2)scrollRect.transform.InverseTransformPoint(obj.position);
    //    //var scrollHeight = scrollRect.GetComponent<RectTransform>().rect.height;
    //    var scrollHeight = contentHeight.rect.height;
    //    var objHeight = obj.rect.height;

    //    if (objPosition.y > scrollHeight / 2)
    //    {
    //        contentPanel.localPosition = new Vector2(contentPanel.localPosition.x,
    //            contentPanel.localPosition.y - objHeight - paddingTop);
    //    }

    //    if (objPosition.y < -scrollHeight / 2)
    //    {
    //        contentPanel.localPosition = new Vector2(contentPanel.localPosition.x,
    //    contentPanel.localPosition.y + objHeight + paddingBottom);
    //    }
    //}

}
