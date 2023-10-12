using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Minigame_ABAlternate : MinigameBase
{
    [SerializeField] private float timeBeforeFail;
    [SerializeField] private float barDecreaseRate;
    [SerializeField] private int increasePerCorrectButtonPress;

    [SerializeField] private Image barFill;

    [SerializeField] private Image btnAIndicator;
    [SerializeField] private Image btnBIndicator;

    private readonly Color COLOR_ACTIVE = new (255, 255, 255);
    private readonly Color COLOR_INACTIVE = new (200, 200, 200);

    private readonly float BAR_MAX = 100.00f;
    private float barCurrent; // Will go 0 - 100
    private MinigameButton nextButton = MinigameButton.A;


    private new void Awake()
    {
        base.Awake();
        barFill.fillAmount = 0f;
    }
    public new void StartMinigame()
    {
        base.StartMinigame();
        btnBIndicator.color = COLOR_INACTIVE;
        StartCoroutine(C_Minigame());
    }

    private IEnumerator C_Minigame()
    {
        float timeElapsed = 0;
        while (timeElapsed <= timeBeforeFail)
        {
            if(nextButton == MinigameButton.A && inputManager.GetMinigameButtonAPressed())
            {
                barCurrent += increasePerCorrectButtonPress;
                nextButton = MinigameButton.B;
                btnAIndicator.color = COLOR_INACTIVE;
                btnBIndicator.color = COLOR_ACTIVE;
            }
            else if(nextButton == MinigameButton.B && inputManager.GetMinigameButtonBPressed())
            {
                barCurrent += increasePerCorrectButtonPress;
                nextButton = MinigameButton.A;
                btnAIndicator.color = COLOR_ACTIVE;
                btnBIndicator.color = COLOR_INACTIVE;
            }
            barCurrent -= barDecreaseRate * Time.deltaTime;
            barFill.fillAmount = barCurrent / BAR_MAX;
            timeElapsed += Time.deltaTime;
            if(barCurrent >= BAR_MAX)
            {
                // Made it to the end
                OnComplete(1);
                yield break;
            }
            yield return null;
        }
        // If reached this point, you waited too long
        OnComplete(0);
    }
}
