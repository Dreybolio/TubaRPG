using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame_SpamA : MinigameBase
{
    [SerializeField] private float timeBeforeFail;
    [SerializeField] private float barDecreaseRate;
    [SerializeField] private int increasePerCorrectButtonPress;

    [SerializeField] private Image barFill;

    [SerializeField] private Image btnAIndicator;
    [SerializeField] private Image successIndicator;

    private readonly Vector2 SCALE_UNPRESSED = new(1, 1);
    private readonly Vector2 SCALE_PRESSED = new(1.25f, 1.25f);
    private bool _beingScaled = false;

    private readonly float BAR_MAX = 100.00f;
    private float barCurrent; // Will go 0 - 100


    private new void Awake()
    {
        base.Awake();
        barFill.fillAmount = 0f;
        btnAIndicator.sprite = sprA;
    }
    public override void StartMinigame()
    {
        StartCoroutine(C_Minigame());
    }

    private IEnumerator C_Minigame()
    {
        float timeElapsed = 0;
        while (timeElapsed <= timeBeforeFail)
        {
            if(inputManager.GetMinigameButtonAPressed())
            {
                barCurrent += increasePerCorrectButtonPress;
            }
            barCurrent -= barDecreaseRate * Time.deltaTime;
            barFill.fillAmount = barCurrent / BAR_MAX;
            timeElapsed += Time.deltaTime;
            if(barCurrent >= BAR_MAX)
            {
                // Made it to the end
                btnAIndicator.enabled = false;
                successIndicator.enabled = true;
                successIndicator.sprite = sprCorrect;
                OnComplete(1);
                yield break;
            }

            // Scale the button to if it's being pressed or not
            if (!_beingScaled && inputManager.GetMinigameButtonAHeld())
            {
                btnAIndicator.transform.localScale = SCALE_PRESSED;
                _beingScaled = true;
            }
            else if(_beingScaled && !inputManager.GetMinigameButtonAHeld())
            {
                btnAIndicator.transform.localScale = SCALE_UNPRESSED;
                _beingScaled = false;
            }
            yield return null;
        }
        // If reached this point, you waited too long
        OnComplete(0);
        btnAIndicator.enabled = false;
        successIndicator.enabled = true;
        successIndicator.sprite = sprIncorrect;
    }
}
