using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Minigame_WaitPressABXY : MinigameBase
{
    [SerializeField] private float minWaitTime;
    [SerializeField] private float maxWaitTime;
    [SerializeField] private float pressTimeout;

    [SerializeField] private TextMeshProUGUI textPress;
    [SerializeField] private TextMeshProUGUI textWaitForIt;
    [SerializeField] private Image indicator;
    private MinigameButton button;

    private float waitTime;
    private new void Awake()
    {
        base.Awake();
        DecideButton();
        SetIndicatorSprite(indicator, button);
    }
    public override void StartMinigame()
    {
        waitTime = Random.Range(minWaitTime, maxWaitTime);
        textPress.enabled = false;
        indicator.enabled = false;
        StartCoroutine(C_Minigame());
    }
    private IEnumerator C_Minigame()
    {
        // WaitForIt is enabled
        float timeElapsed = 0;
        while(timeElapsed < waitTime)
        {
            if(GetButtonPressed() != MinigameButton.NONE)
            {
                // Fail Minigame
                indicator.sprite = sprIncorrect;
                indicator.enabled = true;
                textWaitForIt.enabled = false;
                OnComplete(0);
                yield break;
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        indicator.enabled = true;
        textWaitForIt.enabled = false;
        textPress.enabled = true;
        timeElapsed = 0;
        while (timeElapsed < pressTimeout)
        {
            if(GetButtonPressed() != MinigameButton.NONE)
            {
                if (GetButtonPressed() == button)
                {
                    // Succeed Minigame
                    indicator.sprite = sprCorrect;
                    textPress.enabled = false;
                    OnComplete(1);
                    yield break;
                }
                else
                {
                    // Pressed wrong button.
                    indicator.sprite = sprIncorrect;
                    textPress.enabled = false;
                    OnComplete(0);
                    yield break;
                }
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        // Waited too long.
        indicator.sprite = sprIncorrect;
        indicator.enabled = true;
        textPress.enabled = false;
        OnComplete(0);
        yield break;

    }
    private void DecideButton()
    {
        int r1 = Random.Range(1, 5);
        button = (MinigameButton)r1; // A, B, X, Y
    }
}
