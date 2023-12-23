using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame_5ButtonTimed : MinigameBase
{
    [SerializeField] private float fillTime;
    [SerializeField] private Image barFill;
    [SerializeField] private Image[] btnImages;

    private readonly float[] pressPoints = new float[] {17, 37, 57, 77, 97};
    private readonly float leniency = 6.5f;
    private readonly float BAR_MAX = 100.00f;
    private float barCurrent;

    private readonly Color COLOR_ACTIVE = new(1, 1, 1);
    private readonly Color COLOR_INACTIVE = new(0.5f, 0.5f, 0.5f);

    private MinigameButton[] btnOrder;
    protected new void Awake()
    {
        base.Awake();
        barFill.fillAmount = 0;
    }
    public override void StartMinigame()
    {
        DecideOrder();
        btnImages[0].color = COLOR_ACTIVE;
        btnImages[1].color = COLOR_INACTIVE;
        btnImages[2].color = COLOR_INACTIVE;
        btnImages[3].color = COLOR_INACTIVE;
        btnImages[4].color = COLOR_INACTIVE;
        StartCoroutine(C_Minigame());
    }
    private IEnumerator C_Minigame()
    {
        float timeElapsed = 0;
        int index = 0;
        while (timeElapsed < fillTime)
        {
            barCurrent = Mathf.Lerp(0, BAR_MAX, timeElapsed / fillTime);
            barFill.fillAmount = barCurrent / BAR_MAX;
            timeElapsed += Time.deltaTime;
            MinigameButton pressedButton = GetButtonPressed();
            if(pressedButton != MinigameButton.NONE)
            {
                if(barCurrent > pressPoints[index] - leniency && barCurrent < pressPoints[index] + leniency)
                {
                    // Within correct space
                    if(pressedButton == btnOrder[index])
                    {
                        // Correct Button Is Pressed!
                        btnImages[index].sprite = sprCorrect;
                        if (index == 4)
                        {
                            // Minigame Win :D
                            OnComplete(1);
                            yield break;
                        }
                        else
                        {
                            // Not won yet...
                            btnImages[index + 1].color = COLOR_ACTIVE;
                            index++;
                        }
                    }
                }
                else
                {
                    // Fail! Too Early.
                    btnImages[index].sprite = sprIncorrect;
                    OnComplete(0);
                    yield break;
                }
            }
            if(barCurrent > pressPoints[index] + leniency)
            {
                // Fail! Too late.
                btnImages[index].sprite = sprIncorrect;
                OnComplete(0);
                yield break;
            }
            yield return null;
        }
        // Waited too long
        btnImages[index].sprite = sprIncorrect;
        OnComplete(0);
        yield break;
    }
    private void DecideOrder()
    {
        btnOrder = new MinigameButton[5];
        for (int i = 0; i < 5; i++)
        {
            // 1 is A, 2 is B, 3 is X, 4 is Y
            // 5 is UP, 6 is DOWN, 7 is LEFT, 8 is RIGHT
            int r = Random.Range(1, 9);
            btnOrder[i] = (MinigameButton)r;
            SetIndicatorSprite(btnImages[i], btnOrder[i]);
        }
    }
}
