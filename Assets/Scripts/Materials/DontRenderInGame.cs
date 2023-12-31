using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DontRenderInGame : MonoBehaviour
{
    private void Awake()
    {
        if(TryGetComponent(out Renderer r))
        {
            r.enabled = false;
        }
    }
}
