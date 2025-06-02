using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityExtentions
{
    public static bool LayerInLayerMask(LayerMask mask, int layer)
    {
        return mask == (mask | 1 << layer);
    }
    public static void IgnoreAllPhysicsBetweenLayers(LayerMask[] masks)
    {
        for (int i = 0; i < masks.Length; i++)
        {
            for (int j = 0; j < masks.Length; j++)
            {
                Physics.IgnoreLayerCollision(masks[i], masks[j]);
            }
        }
    }
    public static bool RectOverlaps(this RectTransform a, RectTransform b)
    {
        return a.TransformToWorldRect().Overlaps(b.TransformToWorldRect(), true);
    }

    public static Rect TransformToWorldRect(this RectTransform rectTransform)
    {
        Vector2 sizeDelta = rectTransform.sizeDelta;
        float rectTransformWidth = sizeDelta.x * rectTransform.lossyScale.x;
        float rectTransformHeight = sizeDelta.y * rectTransform.lossyScale.y;

        Vector3 position = rectTransform.position;
        return new Rect(position.x - rectTransformWidth / 2f, position.y - rectTransformHeight / 2f, rectTransformWidth, rectTransformHeight);
    }
}
