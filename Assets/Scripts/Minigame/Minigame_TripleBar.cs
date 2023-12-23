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

    [SerializeField] private RectTransform[] barPressPointTransforms;
    [SerializeField] private Image[] barPressPointImages;
    [SerializeField] private Image[] barButtonIndicators;

    [SerializeField] private Image[] barFills;

    private readonly float BAR_MAX = 100.00f;
    private float[] barCurrent = new float[3];

    // Three bars with different points
    private float[] barPressPoints = new float[3];
    private readonly float leniency = 3.5f;

    // This might be used? I don't know.
    //private bool barRoutineDone = false; 

    private MinigameButton[] barButtons = new MinigameButton[3];


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
            barPressPoints[i] = Random.Range(25 + leniency, (int)BAR_MAX - leniency - 25);
            barPressPointTransforms[i].anchoredPosition = new Vector2(Mathf.Lerp(xPosMin, xPosMax, barPressPoints[i] / BAR_MAX), barPressPointTransforms[i].anchoredPosition.y);

        }
        StartCoroutine(C_Minigame());
    }

    private IEnumerator C_Minigame()
    {
        int i = 0;
        while(i < 3)
        {
            float timeElapsed = 0;
            while (timeElapsed <= fillTime)
            {
                barCurrent[i] = Mathf.Lerp(0, BAR_MAX, timeElapsed / fillTime);
                barFills[i].fillAmount = barCurrent[i] / BAR_MAX;

                float timeMultiplier = 0.5f + (0.5f * (i + 1));
                // 1 if i = 0,    1.5 if i = 1,    2 if i = 2
                timeElapsed += Time.deltaTime * timeMultiplier;
                MinigameButton pressedButton = GetButtonPressed();
                if (pressedButton != MinigameButton.NONE)
                {
                    if (barCurrent[i] > barPressPoints[i] - leniency && barCurrent[i] < barPressPoints[i] + leniency)
                    {
                        // Within correct space
                        if (pressedButton == barButtons[i])
                        {
                            // Correct Button Is Pressed!
                            barPressPointImages[i].sprite = sprCorrect;
                            if (i < 2)
                            {
                                // Increment to next loop
                                yield return new WaitForSeconds(0.15f);
                                i++;
                                timeElapsed = 0;
                                continue;
                            }
                            else
                            {
                                // Minigame Win :D
                                OnComplete(1);
                                yield break;
                            }
                        }
                    }
                    else
                    {
                        // Fail! Too Early.
                        barPressPointImages[i].sprite = sprIncorrect;
                        OnComplete(0);
                        yield break;
                    }
                }
                if (barCurrent[i] > barPressPoints[i] + leniency)
                {
                    // Fail! Too late.
                    barPressPointImages[i].sprite = sprIncorrect;
                    OnComplete(0);
                    yield break;
                }
                yield return null;
            }
            // Waited too long
            barPressPointImages[i].sprite = sprIncorrect;
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
}
