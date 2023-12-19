using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Minigame_Spin : MinigameBase
{
    [SerializeField] private float timeBeforeFail;
    [SerializeField] private float barDecreaseRate;
    [SerializeField] private int increasePerCorrectButtonPress;

    [SerializeField] private Image barFill;

    [SerializeField] private Image btnIndicator;
    [SerializeField] private TextMeshProUGUI textStartSpinning;

    private readonly float BAR_MAX = 100.00f;
    private float barCurrent; // Will go 0 - 100
    private MinigameButton nextButton = MinigameButton.NONE;


    private new void Awake()
    {
        base.Awake();
        barFill.fillAmount = 0f;
        btnIndicator.sprite = sprUp;
    }
    public override void StartMinigame()
    {
        btnIndicator.enabled = false;
        textStartSpinning.enabled = true;
        StartCoroutine(C_Minigame());
    }

    private IEnumerator C_Minigame()
    {
        float flashTimer = 0.40f;
        float time = 0;
        bool flashToggle = true;
        // Get the first direction (Decided by first player input)
        while(nextButton == MinigameButton.NONE)
        {
            if(GetButtonPressed() == MinigameButton.UP)
            {
                nextButton = MinigameButton.UP;
                btnIndicator.sprite = sprUp;
            }
            else if (GetButtonPressed() == MinigameButton.DOWN)
            {
                nextButton = MinigameButton.DOWN;
                btnIndicator.sprite = sprDown;
            }
            else if (GetButtonPressed() == MinigameButton.LEFT)
            {
                nextButton = MinigameButton.LEFT;
                btnIndicator.sprite = sprLeft;
            }
            else if (GetButtonPressed() == MinigameButton.RIGHT)
            {
                nextButton = MinigameButton.RIGHT;
                btnIndicator.sprite = sprRight;
            }
            // Handle text flashing
            if (flashToggle)
            {
                if(time > flashTimer)
                {
                    flashToggle = false;
                    textStartSpinning.enabled = false;
                }
                else
                {
                    time += Time.deltaTime;
                }
            }
            else
            {
                if (time < 0)
                {
                    flashToggle = true;
                    textStartSpinning.enabled = true;
                }
                else
                {
                    time -= Time.deltaTime;
                }
            }
            yield return null;
        }
        textStartSpinning.enabled = false;
        btnIndicator.enabled = true;
        float timeElapsed = 0;
        while (timeElapsed <= timeBeforeFail)
        {
            if(nextButton == MinigameButton.UP && inputManager.GetMinigameButtonUpPressed())
            {
                barCurrent += increasePerCorrectButtonPress;
                nextButton = MinigameButton.RIGHT;
                btnIndicator.sprite = sprRight;
            }
            else if(nextButton == MinigameButton.RIGHT && inputManager.GetMinigameButtonRightPressed())
            {
                barCurrent += increasePerCorrectButtonPress;
                nextButton = MinigameButton.DOWN;
                btnIndicator.sprite = sprDown;
            }
            else if (nextButton == MinigameButton.DOWN && inputManager.GetMinigameButtonDownPressed())
            {
                barCurrent += increasePerCorrectButtonPress;
                nextButton = MinigameButton.LEFT;
                btnIndicator.sprite = sprLeft;
            }
            else if (nextButton == MinigameButton.LEFT && inputManager.GetMinigameButtonLeftPressed())
            {
                barCurrent += increasePerCorrectButtonPress;
                nextButton = MinigameButton.UP;
                btnIndicator.sprite = sprUp;
            }
            barCurrent -= barDecreaseRate * Time.deltaTime;
            barFill.fillAmount = barCurrent / BAR_MAX;
            timeElapsed += Time.deltaTime;
            if(barCurrent >= BAR_MAX)
            {
                // Made it to the end
                btnIndicator.sprite = sprCorrect;
                OnComplete(1);
                yield break;
            }
            yield return null;
        }
        // If reached this point, you waited too long
        OnComplete(0);
        btnIndicator.sprite = sprIncorrect;
    }
}
