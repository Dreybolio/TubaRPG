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
}
