using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

public class MinigameSelectEntity : MinigameBase
{
    [SerializeField] private RectTransform selector;
    [SerializeField] private RectTransform selectorHitbox;
    [SerializeField] private RectTransform point1, point2, point3;
    [SerializeField] private Image point1Img, point2Img, point3Img;
    [SerializeField] private float selectorSpeed = 1.0f;

    private bool _point1Pressed, _point2Pressed, _point3Pressed;
    public override void StartMinigame()
    {
        StartCoroutine(C_Minigame());
    }
    private IEnumerator C_Minigame()
    {
        while (!_point1Pressed || !_point2Pressed || !_point3Pressed)
        {
            Vector2 moveInput = GetInput();
            print("Move Input: " +  moveInput);
            selector.anchoredPosition += selectorSpeed * Time.deltaTime * moveInput;

            if (CheckButtonIsPressed(MinigameButton.A))
            {
                print("Checking for RectOverlaps...");
                if(!_point1Pressed && UnityExtentions.RectOverlaps(selectorHitbox, point1))
                {
                    _point1Pressed = true;
                    point1Img.sprite = sprCorrect;
                    print("Found 1!");
                }
                if (!_point2Pressed && UnityExtentions.RectOverlaps(selectorHitbox, point2))
                {
                    _point2Pressed = true;
                    point2Img.sprite = sprCorrect;
                    print("Found 2!");
                }
                if (!_point3Pressed && UnityExtentions.RectOverlaps(selectorHitbox, point3))
                {
                    _point3Pressed = true;
                    point3Img.sprite = sprCorrect;
                    print("Found 3!");
                }
            }
            yield return null;
        }
        OnComplete(1);
        yield break;
    }
    public Vector2 GetInput()
    {
        Vector2 v = Vector2.zero;
        if (CheckButtonIsHeld(MinigameButton.UP)) { v.y += 1; }
        if (CheckButtonIsHeld(MinigameButton.DOWN)) { v.y -= 1; }
        if (CheckButtonIsHeld(MinigameButton.RIGHT)) { v.x += 1; }
        if (CheckButtonIsHeld(MinigameButton.LEFT)) { v.x -= 1; }
        v.Normalize();
        return v;
    }
}
