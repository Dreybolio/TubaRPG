using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Minigame_TripleBar : MinigameBase
{
    [SerializeField] private float fillTime;

    [SerializeField] private float xPosMin;
    [SerializeField] private float xPosMax;

    [SerializeField] private RectTransform[][] barPressPointTransforms;
    [SerializeField] private Image[][] barPressPointImages;
    [SerializeField] private Image[] barButtonIndicators;

    [SerializeField] private Image[] barFills;

    private readonly float BAR_MAX = 100.00f;
    private float[] barCurrent = new float[] {0, 0, 0};

    // Three sets of two
    private readonly float[][] barPressPoints = new float[][] {new float[] {0, 0}, new float[] { 0, 0 }, new float[] { 0, 0 } };
    private readonly float leniency = 3;

    // This might be used? I don't know.
    //private bool barRoutineDone = false; 

    private MinigameButton[] barButtons;


    private new void Awake()
    {
        base.Awake();
        for(int i = 0; i < 3; i++)
        {
            barFills[i].fillAmount = 0f;
        }
    }
    public override void StartMinigame()
    {
        DecideButtons();
        for(int i = 0; i < 3; i++)
        {
            SetIndicatorSprite(barButtonIndicators[i], barButtons[i]);
            barPressPoints[i][0] = Random.Range(25 + leniency, (int)BAR_MAX - leniency - 25);
            do
            {
                // Try to set the second point, must be at least 5 away from the first.
                float chosenPoint = Random.Range(barPressPoints[i][0] + leniency, (int)BAR_MAX - leniency);
                if (Mathf.Abs(barPressPoints[i][1] - chosenPoint) > 5)
                {
                    barPressPoints[i][1] = chosenPoint;
                }
            } while (barPressPoints[i][1] == 0);
            for(int j = 0; j < 2; j++)
            {
                barPressPointTransforms[i][j].anchoredPosition = new Vector2(Mathf.Lerp(xPosMin, xPosMax, barPressPoints[i][j] / BAR_MAX), barPressPointTransforms[i][j].anchoredPosition.y);
            }

        }
        StartCoroutine(C_Minigame());
    }

    private IEnumerator C_Minigame()
    {
        int i = 0;
        while(i < 3)
        {
            float timeElapsed = 0;
            int j = 0;
            while (timeElapsed <= fillTime)
            {
                barCurrent[i] = Mathf.Lerp(0, BAR_MAX, timeElapsed / fillTime);
                barFills[i].fillAmount = barCurrent[i] / BAR_MAX;
                timeElapsed += Time.deltaTime;
                MinigameButton pressedButton = GetButtonPressed();
                if (pressedButton != MinigameButton.NONE)
                {
                    if (barCurrent[i] > barPressPoints[i][j] - leniency && barCurrent[i] < barPressPoints[i][j] + leniency)
                    {
                        // Within correct space
                        if (pressedButton == barButtons[i])
                        {
                            // Correct Button Is Pressed!
                            barPressPointImages[i][j].sprite = sprCorrect;
                            if (j == 0)
                            {
                                // Stay on this bar, but we're now looking for the next one.
                                j++;
                            }
                            else
                            {
                                // j is 1
                                if (i < 2)
                                {
                                    // Increment to next loop
                                    i++;
                                    continue;
                                }
                                else
                                {
                                    // Minigame Win :D
                                    OnComplete(1);
                                    yield break;
                                }
                                // Minigame Win :D
                            }
                        }
                    }
                    else
                    {
                        // Fail! Too Early.
                        barPressPointImages[i][j].sprite = sprIncorrect;
                        OnComplete(0);
                        yield break;
                    }
                }
                if (barCurrent[i] > barPressPoints[i][j] + leniency)
                {
                    // Fail! Too late.
                    barPressPointImages[i][j].sprite = sprIncorrect;
                    OnComplete(0);
                    yield break;
                }
                yield return null;
            }
            // Waited too long
            barPressPointImages[i][j].sprite = sprIncorrect;
            OnComplete(0);
            yield break;
        }
    }

    private void DecideButtons()
    {
        int r1 = Random.Range(1, 5);
        barButtons[0] = (MinigameButton)r1; // A, B, X, Y
        int r2 = Random.Range(1, 5);
        barButtons[1] = (MinigameButton)r2; // A, B, X, Y
        int r3 = Random.Range(1, 5);
        barButtons[2] = (MinigameButton)r3; // A, B, X, Y
    }
    private void SetIndicatorSprite(Image indicator, MinigameButton type)
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
        }
    }
    private MinigameButton GetButtonPressed()
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
}
