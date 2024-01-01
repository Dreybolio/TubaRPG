using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Hitbox : MonoBehaviour
{
    public delegate void CollisionEvent();
    public event CollisionEvent OnHitboxEnter;
    [SerializeField] private LayerMask triggerableLayers;
    private void OnTriggerEnter(Collider other)
    {
        if(UnityExtentions.LayerInLayerMask(triggerableLayers, other.gameObject.layer))
        {
            OnHitboxEnter?.Invoke();
        }
    }
}
