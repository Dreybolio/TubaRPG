using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraSettings : MonoBehaviour
{
    Camera cam;
    private void Awake()
    {
        cam = GetComponent<Camera>();
    }
    private void Start()
    {
        cam.transparencySortMode = TransparencySortMode.Orthographic;
    }
}
