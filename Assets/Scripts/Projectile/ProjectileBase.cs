using System;
using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    [NonSerialized] public bool finishedMovement = false;

    public abstract void MoveToTarget(Vector3 target, float time);
    public abstract void Destroy();
}
