using DG.Tweening;
using UnityEngine;

public static class RectTransformExtensions
{
    public static void ExpandOverCanvas(this RectTransform p_rectTransform)
    {
        p_rectTransform.pivot = new Vector2(0.5f, 0.5f);
        p_rectTransform.anchorMin = Vector2.zero;
        p_rectTransform.anchorMax = Vector2.one;
    }

    public static void SetVerticalPosition(this RectTransform p_rectTransform, float p_bottomToTop = 0.5f, bool p_changePivot = true)
    {
        p_rectTransform.anchorMax = new Vector2(p_rectTransform.anchorMax.x, p_bottomToTop);
        p_rectTransform.anchorMin = new Vector2(p_rectTransform.anchorMin.x, p_bottomToTop);

        if (p_changePivot)
        {
            p_rectTransform.pivot = new Vector2(p_rectTransform.pivot.x, p_bottomToTop);
        }

        p_rectTransform.anchoredPosition = Vector2.zero;
    }

    public static void DOVerticalPosition(this RectTransform p_rectTransform, float p_duration, float p_bottomToTop = 0.5f, bool p_changePivot = true)
    {
        p_rectTransform.DOAnchorMax(new Vector2(p_rectTransform.anchorMax.x, p_bottomToTop), p_duration);
        p_rectTransform.DOAnchorMin(new Vector2(p_rectTransform.anchorMin.x, p_bottomToTop), p_duration);

        if (p_changePivot)
        {
            p_rectTransform.DOPivotY(p_bottomToTop, p_duration);
        }

        p_rectTransform.DOAnchorPos(Vector2.zero, p_duration);
    }
}