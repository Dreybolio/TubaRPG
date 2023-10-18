using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Minigame_DoubleHoldRelease : MinigameBase
{
    [SerializeField] private float fillTime;

    [SerializeField] private float xPosMin;
    [SerializeField] private float xPosMax;
    [SerializeField] private float successWidth;

    [SerializeField] private RectTransform bar1ReleasePoint;
    [SerializeField] private Image bar1ReleasePointImage;
    [SerializeField] private Image bar1ButtonIndicator;
    [SerializeField] private RectTransform bar2ReleasePoint;
    [SerializeField] private Image bar2ReleasePointImage;
    [SerializeField] private Image bar2ButtonIndicator;

    [SerializeField] private Image bar1Fill;
    [SerializeField] private Image bar2Fill;

    private readonly float BAR_MAX = 100.00f;
    private float bar1Current;
    private float bar2Current;

    private int bar1DesiredRelease;
    private int bar2DesiredRelease;
    private int leniency;

    private bool bar1RoutineDone = false;
    private bool bar2RoutineDone = false;
    private int routineSuccess = 0;

    private MinigameButton bar1Button;
    private MinigameButton bar2Button;


    private new void Awake()
    {
        base.Awake();
        bar1Fill.fillAmount = 0f;
        bar2Fill.fillAmount = 0f;
    }
    public override void StartMinigame()
    {
        DecideButtons();
        SetIndicatorSprite(bar1ButtonIndicator, bar1Button);
        SetIndicatorSprite(bar2ButtonIndicator, bar2Button);
        leniency = Mathf.RoundToInt(successWidth / Mathf.Abs(xPosMax - xPosMin) * BAR_MAX / 2); // width-to-full ratio applied to a 0-100 scale, halved.
        bar1DesiredRelease = Random.Range(50 + leniency, (int)BAR_MAX - leniency);
        bar1ReleasePoint.anchoredPosition = new Vector2(Mathf.Lerp(xPosMin, xPosMax, bar1DesiredRelease / BAR_MAX), bar1ReleasePoint.anchoredPosition.y);
        bar2DesiredRelease = Random.Range(50 + leniency, (int)BAR_MAX - leniency);
        bar2ReleasePoint.anchoredPosition = new Vector2(Mathf.Lerp(xPosMin, xPosMax, bar2DesiredRelease / BAR_MAX), bar2ReleasePoint.anchoredPosition.y);
        StartCoroutine(C_Minigame());
    }

    private IEnumerator C_Minigame()
    {
        yield return new WaitUntil(() => CheckButtonIsHeld(bar1Button) && CheckButtonIsHeld(bar2Button));
        StartCoroutine(C_SubroutineBar1());
        StartCoroutine(C_SubroutineBar2());
        yield return new WaitUntil(() => bar1RoutineDone);
        yield return new WaitUntil(() => bar2RoutineDone);
        OnComplete(routineSuccess);
    }
    private IEnumerator C_SubroutineBar1() 
    {
        float startPos = 0;
        float timeElapsed = 0;
        while (timeElapsed <= fillTime)
        {
            bar1Current = Mathf.Lerp(startPos, BAR_MAX, timeElapsed / fillTime);
            bar1Fill.fillAmount = bar1Current / BAR_MAX;
            timeElapsed += Time.deltaTime;
            if(!CheckButtonIsHeld(bar1Button))
            {
                print("Desired Release: " + bar1DesiredRelease + ", Current: " + bar1Current + ", leniency: " + leniency);
                // Player let go.
                if(bar1Current > bar1DesiredRelease - leniency && bar1Current < bar1DesiredRelease + leniency)
                {
                    // Player let go correctly!
                    bar1ReleasePointImage.sprite = sprCorrect;
                    bar1RoutineDone = true;
                    routineSuccess++;
                    yield break;
                }
                else
                {
                    bar1ReleasePointImage.sprite = sprIncorrect;
                    bar1RoutineDone = true;
                    yield break;
                }
            }
            yield return null;
        }
        // If reached this point, you waited too long
        bar1Fill.fillAmount = 1;
        bar1ReleasePointImage.sprite = sprIncorrect;
        bar1RoutineDone = true;
    }
    private IEnumerator C_SubroutineBar2()
    {
        float startPos = 0;
        float timeElapsed = 0;
        while (timeElapsed <= fillTime)
        {
            bar2Current = Mathf.Lerp(startPos, BAR_MAX, timeElapsed / fillTime);
            bar2Fill.fillAmount = bar2Current / BAR_MAX;
            timeElapsed += Time.deltaTime;
            if (!CheckButtonIsHeld(bar2Button))
            {
                // Player let go.
                if (bar2Current > bar2DesiredRelease - leniency && bar2Current < bar2DesiredRelease + leniency)
                {
                    // Player let go correctly!
                    bar2ReleasePointImage.sprite = sprCorrect;
                    bar2RoutineDone = true;
                    routineSuccess++;
                    yield break;
                }
                else
                {
                    bar2ReleasePointImage.sprite = sprIncorrect;
                    bar2RoutineDone = true;
                    yield break;
                }
            }
            yield return null;
        }
        // If reached this point, you waited too long
        bar2Fill.fillAmount = 1;
        bar2ReleasePointImage.sprite = sprIncorrect;
        bar2RoutineDone = true;
    }

    private void DecideButtons()
    {
        int r1 = Random.Range(1, 5);
        bar1Button = (MinigameButton)r1; // A, B, X, Y
        int r2;
        do
        {
            r2 = Random.Range(1, 5);
        } while (r2 == r1); // Loop until r2 is not r1
        bar2Button = (MinigameButton)r2;
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
    private bool CheckButtonIsHeld(MinigameButton btn)
    {
        return btn switch
        {
            MinigameButton.A => inputManager.GetMinigameButtonAHeld(),
            MinigameButton.B => inputManager.GetMinigameButtonBHeld(),
            MinigameButton.X => inputManager.GetMinigameButtonXHeld(),
            MinigameButton.Y => inputManager.GetMinigameButtonYHeld(),
            _ => throw new System.NotImplementedException()
        };
    }
}
