using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Minigame_WaitPressA : MinigameBase
{
    [SerializeField] private float minWaitTime;
    [SerializeField] private float maxWaitTime;
    [SerializeField] private float pressTimeout;

    [SerializeField] private TextMeshProUGUI textPress;
    [SerializeField] private TextMeshProUGUI textWaitForIt;
    [SerializeField] private Image indicator;

    private float waitTime;
    private new void Awake()
    {
        base.Awake();
        indicator.sprite = sprA;
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
            if(inputManager.GetMinigameButtonAPressed() )
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
            if (inputManager.GetMinigameButtonAPressed())
            {
                // Succeed Minigame
                indicator.sprite = sprCorrect;
                textPress.enabled = false;
                OnComplete(1);
                yield break;
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
}
