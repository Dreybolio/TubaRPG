using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame_5Button : MinigameBase
{
    [SerializeField] private float timeout;
    [SerializeField] private Image barFill;
    [SerializeField] private Image[] btnImages;

    private readonly float BAR_MAX = 100.00f;
    private float barCurrent;

    private readonly Color COLOR_ACTIVE = new(1, 1, 1);
    private readonly Color COLOR_INACTIVE = new(0.5f, 0.5f, 0.5f);

    private MinigameButton[] btnOrder;
    private new void Awake()
    {
        base.Awake();
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
        while(timeElapsed < timeout)
        {
            barCurrent = Mathf.Lerp(BAR_MAX, 0, timeElapsed / timeout); // From 100 - 0
            barFill.fillAmount = barCurrent / BAR_MAX;
            timeElapsed += Time.deltaTime;
            MinigameButton pressedButton = GetButtonPressed();
            if(pressedButton != MinigameButton.NONE)
            {
                if(pressedButton == btnOrder[index])
                {
                    // Pressed correct button :)
                    btnImages[index].sprite = sprCorrect;
                    if(index == 4)
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
                else
                {
                    // Pressed incorrect button :(
                    btnImages[index].sprite = sprIncorrect;
                    OnComplete(0);
                    yield break;
                }
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
            int r = Random.Range(1, 5); //1 is A, 2 is B, 3 is X, 4 is Y
            btnOrder[i] = (MinigameButton)r;
            SetIndicatorSprite(btnImages[i], btnOrder[i]);
        }
    }
}
