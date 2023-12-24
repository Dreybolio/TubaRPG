using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonIndicator : MonoBehaviour
{
    [SerializeField] private MinigameButton button;
    [SerializeField] private Image btnIndicator;
    private InputManager inputManager;
    void Start()
    {
        inputManager = InputManager.Instance;
        SetSprite(button);
    }
    public void SetSprite(MinigameButton button)
    {
        Sprite[] sprites = inputManager.GetInputSprites();
        switch (button)
        {
            case MinigameButton.NONE:
                btnIndicator.enabled = false;
                break;
            case MinigameButton.A:
                btnIndicator.sprite = sprites[0];
                break;
            case MinigameButton.B:
                btnIndicator.sprite = sprites[1];
                break;
            case MinigameButton.X:
                btnIndicator.sprite = sprites[2];
                break;
            case MinigameButton.Y:
                btnIndicator.sprite = sprites[3];
                break;
            case MinigameButton.UP:
                btnIndicator.sprite = sprites[4];
                break;
            case MinigameButton.DOWN:
                btnIndicator.sprite = sprites[5];
                break;
            case MinigameButton.LEFT:
                btnIndicator.sprite = sprites[6];
                break;
            case MinigameButton.RIGHT:
                btnIndicator.sprite = sprites[7];
                break;
        }
    }
}
