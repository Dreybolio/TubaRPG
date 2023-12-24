using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLinear : ProjectileBase
{
    public override void MoveToTarget(Vector3 target, float time)
    {
        StartCoroutine(C_MoveToTarget(target, time));
    }
    private IEnumerator C_MoveToTarget(Vector3 target, float time)
    {
        Vector3 startPos = transform.position;
        float timeElapsed = 0;
        while(timeElapsed < time)
        {
            transform.position = Vector3.Lerp(startPos, target, timeElapsed / time);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = target;
        finishedMovement = true;
    }
    public override void Destroy()
    {
        Destroy(gameObject);
    }
}
