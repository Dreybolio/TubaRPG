using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Note: These minigame buttons assume an XBOX controller. This won't always be the case, but it can be understood this way in code.
public enum MinigameButton
{
    A,
    B,
    X,
    Y,
    UP,
    DOWN,
    LEFT,
    RIGHT
}
public class MinigameBase : MonoBehaviour
{
    [NonSerialized] public bool isComplete = false;
    [NonSerialized] public bool success = false;

    protected InputManager inputManager;
    protected void Awake()
    {
        inputManager = InputManager.Instance;
    }
    protected void OnComplete(bool succeeded)
    {
        isComplete = true;
        success = succeeded;
    }
    public void StartMinigame()
    {

    }
    public void Destroy()
    {
        // Do something cooler here eventually
        Destroy(gameObject);
    }
}
