using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Quality : object
{
    private static int targetFrameRate = 60;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void LimitFPS()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
    }
}
