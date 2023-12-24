using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame_HoldReleaseA : MinigameBase
{
    [SerializeField] private float fillTime;

    [SerializeField] private float xPosMin;
    [SerializeField] private float xPosMax;

    [SerializeField] private RectTransform barReleasePoint;
    [SerializeField] private Image barReleasePointImage;
    [SerializeField] private Image btnAIndicator;

    [SerializeField] private Image barFill;

    private readonly float BAR_MAX = 100.00f;
    private float barCurrent;

    private float barDesiredRelease;
    private readonly float leniency = 3.5f;

    private new void Awake()
    {
        base.Awake();
        barFill.fillAmount = 0f;
        btnAIndicator.sprite = sprA;
    }
    public override void StartMinigame()
    {
        barDesiredRelease = 80;
        barReleasePoint.anchoredPosition = new Vector2(Mathf.Lerp(xPosMin, xPosMax, barDesiredRelease / BAR_MAX), barReleasePoint.anchoredPosition.y);
        StartCoroutine(C_Minigame());
    }

    private IEnumerator C_Minigame()
    {
        float startPos = 0;
        float timeElapsed = 0;
        yield return new WaitUntil(() => CheckButtonIsHeld(MinigameButton.A));
        while (timeElapsed <= fillTime)
        {
            barCurrent = Mathf.Lerp(startPos, BAR_MAX, timeElapsed / fillTime);
            barFill.fillAmount = barCurrent / BAR_MAX;
            timeElapsed += Time.deltaTime;
            if (!CheckButtonIsHeld(MinigameButton.A))
            {
                // Player let go.
                if (barCurrent > barDesiredRelease - leniency && barCurrent < barDesiredRelease + leniency)
                {
                    // Player let go correctly!
                    barReleasePointImage.sprite = sprCorrect;
                    OnComplete(1);
                    yield break;
                }
                else
                {
                    barReleasePointImage.sprite = sprIncorrect;
                    OnComplete(0);
                    yield break;
                }
            }
            yield return null;
        }
        // If reached this point, you waited too long
        barFill.fillAmount = 1;
        barReleasePointImage.sprite = sprIncorrect;
        OnComplete(0);
    }
}
