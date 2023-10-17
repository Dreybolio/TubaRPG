using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Note: These minigame buttons assume an XBOX controller. This won't always be the case, but it can be understood this way in code.
public enum MinigameButton
{
    NONE,
    A,
    B,
    X,
    Y,
    UP,
    DOWN,
    LEFT,
    RIGHT
}
public abstract class MinigameBase : MonoBehaviour
{
    [NonSerialized] public bool isComplete = false;
    [NonSerialized] public int successLevel = 0;

    protected Sprite sprA;
    protected Sprite sprB;
    protected Sprite sprX;
    protected Sprite sprY;
    protected Sprite sprUp;
    protected Sprite sprDown;
    protected Sprite sprLeft;
    protected Sprite sprRight;
    [SerializeField] protected Sprite sprCorrect;
    [SerializeField] protected Sprite sprIncorrect;

    protected InputManager inputManager;
    protected void Awake()
    {
        inputManager = InputManager.Instance;
        GetInputButtonSprites();
    }
    protected void OnComplete(int successLevel)
    {
        isComplete = true;
        this.successLevel = successLevel;
    }
    public abstract void StartMinigame();
    public void Destroy()
    {
        // Do something cooler here eventually
        Destroy(gameObject);
    }
    protected void GetInputButtonSprites()
    {
        Sprite[] sprites = inputManager.GetInputSprites();
        sprA = sprites[0];
        sprB = sprites[1];
        sprX = sprites[2];
        sprY = sprites[3];
        sprUp = sprites[4];
        sprDown = sprites[5];
        sprLeft = sprites[6];
        sprRight = sprites[7];
    }
}
