using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    protected void SetIndicatorSprite(Image indicator, MinigameButton type)
    {
        switch (type)
        {
            case MinigameButton.A:
                indicator.sprite = sprA;
                break;
            case MinigameButton.B:
                indicator.sprite = sprB;
                break;
            case MinigameButton.X:
                indicator.sprite = sprX;
                break;
            case MinigameButton.Y:
                indicator.sprite = sprY;
                break;
            case MinigameButton.UP:
                indicator.sprite = sprUp;
                break;
            case MinigameButton.DOWN:
                indicator.sprite = sprDown;
                break;
            case MinigameButton.LEFT:
                indicator.sprite = sprLeft;
                break;
            case MinigameButton.RIGHT:
                indicator.sprite = sprRight;
                break;
        }
    }
    protected MinigameButton GetButtonPressed()
    {
        if (inputManager.GetMinigameButtonAPressed()) { return MinigameButton.A; }
        else if (inputManager.GetMinigameButtonBPressed()) { return MinigameButton.B; }
        else if (inputManager.GetMinigameButtonXPressed()) { return MinigameButton.X; }
        else if (inputManager.GetMinigameButtonYPressed()) { return MinigameButton.Y; }
        else if (inputManager.GetMinigameButtonUpPressed()) { return MinigameButton.UP; }
        else if (inputManager.GetMinigameButtonDownPressed()) { return MinigameButton.DOWN; }
        else if (inputManager.GetMinigameButtonLeftPressed()) { return MinigameButton.LEFT; }
        else if (inputManager.GetMinigameButtonRightPressed()) { return MinigameButton.RIGHT; }
        else { return MinigameButton.NONE; }
    }
    protected bool CheckButtonIsHeld(MinigameButton btn)
    {
        return btn switch
        {
            MinigameButton.A => inputManager.GetMinigameButtonAHeld(),
            MinigameButton.B => inputManager.GetMinigameButtonBHeld(),
            MinigameButton.X => inputManager.GetMinigameButtonXHeld(),
            MinigameButton.Y => inputManager.GetMinigameButtonYHeld(),
            MinigameButton.UP => inputManager.GetMinigameButtonUpHeld(),
            MinigameButton.DOWN => inputManager.GetMinigameButtonDownHeld(),
            MinigameButton.LEFT => inputManager.GetMinigameButtonLeftHeld(),
            MinigameButton.RIGHT => inputManager.GetMinigameButtonRightHeld(),
            _ => throw new NotImplementedException()
        };
    }
}
