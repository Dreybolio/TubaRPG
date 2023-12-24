using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image barFill;
    [SerializeField] private TextMeshProUGUI text;

    CanvasGroup group;

    private int maxValue;
    private void Start()
    {
        group = GetComponent<CanvasGroup>();
    }
    public void Enable()
    {
        group.alpha = 1.0f;
    }
    public void Disable()
    {
        group.alpha = 0.0f;
    }
    public void SetMaximum(int max, bool setToFull = true)
    {
        maxValue = max;
        if(setToFull)
        {
            SetValue(maxValue);
        }
    }
    public void SetValue(int value) 
    {
        text.text = value.ToString();
        float percentage = (float)value / maxValue;
        barFill.fillAmount = percentage;
    }
}
