using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame_HoldLeft : MinigameBase
{
    [SerializeField] private Image[] indicators;
    [SerializeField] private Image btnIndicatorLeft;

    [SerializeField] private Sprite sprPassed;

    private new void Awake()
    {
        base.Awake();
        btnIndicatorLeft.sprite = sprLeft;
    }
    public override void StartMinigame()
    {
        StartCoroutine(C_Minigame());
    }

    private IEnumerator C_Minigame()
    {
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => inputManager.GetMinigameButtonLeftHeld());
        for (int i = 0; i < 5; i++)
        {
            if(i == 0)
            {
                indicators[i].sprite = sprPassed; // Pass 1
                continue;
            }
            float timeElapsed = 0;
            while (timeElapsed < 0.60f)
            {
                timeElapsed += Time.deltaTime;
                if (!inputManager.GetMinigameButtonLeftHeld())
                {
                    if(i != 4)
                    {
                        // Let go too early
                        indicators[3].sprite = sprIncorrect;
                        OnComplete(0);
                        yield break;
                    }
                    else
                    {
                        // Let go on time
                        indicators[3].sprite = sprCorrect;
                        OnComplete(1);
                        yield break;
                    }
                }
                yield return null;
            }
            if(i != 4) // We don't want this to happen when on the last one, as there is no next one
            {
                indicators[i].sprite = sprPassed; // Pass 2/3/4
            }
        }
        // Let go too late
        indicators[3].sprite = sprIncorrect;
        OnComplete(0);
    }
}
