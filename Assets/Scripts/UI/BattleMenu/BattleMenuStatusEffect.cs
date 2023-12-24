using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMenuStatusEffect : MonoBehaviour
{
    [Header("Pointers")]
    [SerializeField] private Image imgIcon;
    [SerializeField] private Image imgNum;

    [Header("Sprites")]
    [SerializeField] private Sprite sprAttackUp;
    [SerializeField] private Sprite sprAttackUpPlus;
    [SerializeField] private Sprite sprDefenceUp;
    [SerializeField] private Sprite sprDefenceUpPlus;
    [SerializeField] private Sprite sprAsleep;
    [SerializeField] private Sprite sprDecrescendo;
    [SerializeField] private Sprite sprFermata;
    [SerializeField] private Sprite sprDizzy;
    [SerializeField] private Sprite[] numSprites;
    public void SetIcon(StatusEffect statusEffect)
    {
        Sprite newSprite = null;
        switch (statusEffect)
        {
            case StatusEffect.ATTACKUP:
                newSprite = sprAttackUp;
                break;
            case StatusEffect.ATTACKUP_PLUS:
                newSprite = sprAttackUpPlus;
                break;
            case StatusEffect.DEFENCEUP:
                newSprite = sprDefenceUp;
                break;
            case StatusEffect.DEFENCEUP_PLUS:
                newSprite = sprDefenceUpPlus;
                break;
            case StatusEffect.ASLEEP: 
                newSprite = sprAsleep;
                break;
            case StatusEffect.DECRESCENDO:
                newSprite = sprDecrescendo;
                break;
            case StatusEffect.FERMATA:
                newSprite = sprFermata;
                break;
            case StatusEffect.DIZZY:
                newSprite = sprDizzy;
                break;
        }
        imgIcon.sprite = newSprite;
    }
    public void SetNumber(int num)
    {
        if(num == -1) { imgNum.enabled = false;  return; } // It is infinite, so no number
        imgNum.sprite = numSprites[num];
    }
}
