using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;

public class ScrollSnapTo : MonoBehaviour
{
    public GameObject parentSpawnUnder;

    public ScrollRect scrollRect;
    public RectTransform content, viewport;
    public List<RectTransform> rectTransforms;
    public float topPos, bottomPos, offSet;
    public int itemCount;
    private RectTransform oldRect;
    private Vector2 originalPos;
    public int offsetGoingUp, offsetGoingDown;

    void OnEnable()
    {
        originalPos = content.anchoredPosition;
    }

    public void ResetButtonSnap()
    {
        content.anchoredPosition = originalPos;
        oldRect = null;
    }

    public void SnapTo(RectTransform target, int index)
    {
        Vector2 offsetVector = new Vector2(0, 0);
        RectTransform rect = rectTransforms[index];
        Vector2 v = rect.position;

        bool inView = RectTransformUtility.RectangleContainsScreenPoint(viewport, v);

        if (!inView)
        {
            if (oldRect != null)
            {
                if (oldRect.localPosition.y < rect.localPosition.y) // if old position was lower than new pos
                {
                    Debug.Log("offsetGoingUp");
                    offsetVector = new Vector2(0, offsetGoingUp);
                }
                else if (oldRect.localPosition.y > rect.localPosition.y)
                {
                    Debug.Log("offsetGoingDown");
                    offsetVector = new Vector2(0, offsetGoingDown);
                }

                Canvas.ForceUpdateCanvases();
                Vector2 targetPosition = (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
                Vector2 contentPosition = (Vector2)scrollRect.transform.InverseTransformPoint(content.position);
                content.anchoredPosition = contentPosition - targetPosition + offsetVector;
            }
        }
        oldRect = rect;
    }

    public void CreateDisplay()
    {
        
    }




}