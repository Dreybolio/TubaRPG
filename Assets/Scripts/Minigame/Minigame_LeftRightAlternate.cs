using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Minigame_LeftRightAlternate : MinigameBase
{
    [SerializeField] private float timeBeforeFail;
    [SerializeField] private float barDecreaseRate;
    [SerializeField] private int increasePerCorrectButtonPress;

    [SerializeField] private Image barFill;

    [SerializeField] private Image btnLeftIndicator;
    [SerializeField] private Image btnRightIndicator;
    [SerializeField] private Image successIndicator;

    private readonly Color COLOR_ACTIVE = new (1, 1, 1);
    private readonly Color COLOR_INACTIVE = new (0.5f, 0.5f, 0.5f);

    private readonly float BAR_MAX = 100.00f;
    private float barCurrent; // Will go 0 - 100
    private MinigameButton nextButton = MinigameButton.LEFT;


    private new void Awake()
    {
        base.Awake();
        barFill.fillAmount = 0f;
        btnLeftIndicator.sprite = sprLeft;
        btnRightIndicator.sprite = sprRight;
    }
    public override void StartMinigame()
    {
        btnRightIndicator.color = COLOR_INACTIVE;
        StartCoroutine(C_Minigame());
    }

    private IEnumerator C_Minigame()
    {
        float timeElapsed = 0;
        while (timeElapsed <= timeBeforeFail)
        {
            if(nextButton == MinigameButton.LEFT && inputManager.GetMinigameButtonLeftPressed())
            {
                barCurrent += increasePerCorrectButtonPress;
                nextButton = MinigameButton.RIGHT;
                btnLeftIndicator.color = COLOR_INACTIVE;
                btnRightIndicator.color = COLOR_ACTIVE;
            }
            else if(nextButton == MinigameButton.RIGHT && inputManager.GetMinigameButtonRightPressed())
            {
                barCurrent += increasePerCorrectButtonPress;
                nextButton = MinigameButton.LEFT;
                btnLeftIndicator.color = COLOR_ACTIVE;
                btnRightIndicator.color = COLOR_INACTIVE;
            }
            barCurrent -= barDecreaseRate * Time.deltaTime;
            barFill.fillAmount = barCurrent / BAR_MAX;
            timeElapsed += Time.deltaTime;
            if(barCurrent >= BAR_MAX)
            {
                // Made it to the end
                btnLeftIndicator.enabled = false;
                btnRightIndicator.enabled = false;
                successIndicator.enabled = true;
                successIndicator.sprite = sprCorrect;
                OnComplete(1);
                yield break;
            }
            yield return null;
        }
        // If reached this point, you waited too long
        OnComplete(0);
        btnLeftIndicator.enabled = false;
        btnRightIndicator.enabled = false;
        successIndicator.enabled = true;
        successIndicator.sprite = sprIncorrect;
    }
}
