using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMenuManager : MonoBehaviour
{
    [Header("Global Data")]
    [SerializeField] private Sprite[] digits; // Digit images from 0-9

    [Header("Repositioning")]
    [SerializeField] private Transform mainSelectionMenu;
    [SerializeField] private Transform[] mainSelectionMenuHeroSnapPoints;

    [Header("Groups")]
    [SerializeField] private CanvasGroup groupMain;
    [SerializeField] private CanvasGroup groupSubAbility;
    [SerializeField] private CanvasGroup groupSubItem;
    [SerializeField] private CanvasGroup groupSubOther;


    [Header("Ability Submenu")]
    [SerializeField] private Text abilityOneText;
    [SerializeField] private Text abilityOneNPCost;
    [SerializeField] private BattleMenuElement_DoAction abilityOneScript;
    [SerializeField] private Text abilityTwoText;
    [SerializeField] private Text abilityTwoNPCost;
    [SerializeField] private BattleMenuElement_DoAction abilityTwoScript;

    [Header("Enemy Selector")]
    [SerializeField] private BattleMenuElement_EnemySelector enemyZeroSelector;
    [SerializeField] private BattleMenuElement_EnemySelector enemyOneSelector;
    [SerializeField] private BattleMenuElement_EnemySelector enemyTwoSelector;

    [Header("Enemy HP Bar")]
    [SerializeField] private CanvasGroup groupE0HP;
    [SerializeField] private CanvasGroup groupE1HP;
    [SerializeField] private CanvasGroup groupE2HP;
    [SerializeField] private Text e0HPText;
    [SerializeField] private Text e1HPText;
    [SerializeField] private Text e2HPText;

    [Header("TopBar - Hero One")]
    [SerializeField] private Image heroOneIcon;
    [SerializeField] private Image[] heroOneMaxHPDigits;
    [SerializeField] private Image[] heroOneHPDigits;
    [SerializeField] private Image[] heroOneMaxNPDigits;
    [SerializeField] private Image[] heroOneNPDigits;

    [Header("TopBar - Hero Two")]
    [SerializeField] private Image heroTwoIcon;
    [SerializeField] private Image[] heroTwoMaxHPDigits;
    [SerializeField] private Image[] heroTwoHPDigits;
    [SerializeField] private Image[] heroTwoMaxNPDigits;
    [SerializeField] private Image[] heroTwoNPDigits;
    public void SetUIVisibility(bool main, bool subAbility, bool subItem, bool subOther)
    {
        groupMain.alpha = main ? 1f : 0f;
        groupSubAbility.alpha = subAbility ? 1f : 0f;
        groupSubItem.alpha = subItem ? 1f : 0f;
        groupSubOther.alpha = subOther ? 1f : 0f;
    }
    public void SetMenuData(GenericHero hero)
    {
        abilityOneText.text = hero.specialOneName;
        abilityOneNPCost.text = hero.specialOneNPCost.ToString();
        abilityOneScript.SetConfirmBehaviour(hero.specialOneRequiresHeroSelection, hero.specialOneRequiresEnemySelection);

        abilityTwoText.text = hero.specialTwoName;
        abilityTwoNPCost.text = hero.specialTwoNPCost.ToString();
        abilityTwoScript.SetConfirmBehaviour(hero.specialTwoRequiresHeroSelection, hero.specialTwoRequiresEnemySelection);
    }
    public void SnapSelectionMenuToHero(int index)
    {
        mainSelectionMenu.position = mainSelectionMenuHeroSnapPoints[index].position;
    }
    public void SetEnemySelectorValidity(int index, bool isValid)
    {
        if(index == 0)
        {
            enemyZeroSelector.validSelection = isValid;
        }
        else if(index == 1)
        {
            enemyOneSelector.validSelection = isValid;
        }
        else if (index == 2)
        {
            enemyTwoSelector.validSelection = isValid;
        }
    }
    public void SetEnemyHPBarValue(int index, int hp, bool enabled = true)
    {
        CanvasGroup group;
        Text text;
        if(index == 0) { group = groupE0HP; text = e0HPText; } // Index 0
        else if (index == 1) { group = groupE1HP; text = e1HPText; } // Index 1
        else { group = groupE2HP; text = e2HPText; } // Index 2
        
        group.alpha = enabled ? 1f : 0f;

        text.text = hp.ToString();
    }
    public void SetHeroOneIcon(Sprite spr)
    {
        heroOneIcon.sprite = spr;
    }
    public void SetHeroOneMaxHP(int maxHP)
    {
        string s = ConvertIntToStringOfLength(maxHP, heroOneMaxHPDigits.Length);
        for (int i = 0; i < heroOneMaxHPDigits.Length; i++)
        {
            int.TryParse(s.Substring(i, 1), out int numAtVal);
            heroOneMaxHPDigits[i].sprite = digits[numAtVal];
        }
    }
    public void SetHeroOneHP(int hp)
    {
        string s = ConvertIntToStringOfLength(hp, heroOneHPDigits.Length);
        for (int i = 0; i < heroOneHPDigits.Length; i++)
        {
            int.TryParse(s.Substring(i, 1), out int numAtVal);
            heroOneHPDigits[i].sprite = digits[numAtVal];
        }
    }
    public void SetHeroOneMaxNP(int hp)
    {
        string s = ConvertIntToStringOfLength(hp, heroOneMaxNPDigits.Length);
        for (int i = 0; i < heroOneMaxNPDigits.Length; i++)
        {
            int.TryParse(s.Substring(i, 1), out int numAtVal);
            heroOneMaxNPDigits[i].sprite = digits[numAtVal];
        }
    }
    public void SetHeroOneNP(int hp)
    {
        string s = ConvertIntToStringOfLength(hp, heroOneNPDigits.Length);
        for (int i = 0; i < heroOneNPDigits.Length; i++)
        {
            int.TryParse(s.Substring(i, 1), out int numAtVal);
            heroOneNPDigits[i].sprite = digits[numAtVal];
        }
    }
    public void SetHeroTwoIcon(Sprite spr)
    {
        heroTwoIcon.sprite = spr;
    }
    public void SetHeroTwoMaxHP(int maxHP)
    {
        string s = ConvertIntToStringOfLength(maxHP, heroTwoMaxHPDigits.Length);
        for (int i = 0; i < heroTwoMaxHPDigits.Length; i++)
        {
            int.TryParse(s.Substring(i, 1), out int numAtVal);
            heroTwoMaxHPDigits[i].sprite = digits[numAtVal];
        }
    }
    public void SetHeroTwoHP(int hp)
    {
        string s = ConvertIntToStringOfLength(hp, heroTwoHPDigits.Length);
        for (int i = 0; i < heroTwoHPDigits.Length; i++)
        {
            int.TryParse(s.Substring(i, 1), out int numAtVal);
            heroTwoHPDigits[i].sprite = digits[numAtVal];
        }
    }
    public void SetHeroTwoMaxNP(int hp)
    {
        string s = ConvertIntToStringOfLength(hp, heroTwoMaxNPDigits.Length);
        for (int i = 0; i < heroTwoMaxNPDigits.Length; i++)
        {
            int.TryParse(s.Substring(i, 1), out int numAtVal);
            heroTwoMaxNPDigits[i].sprite = digits[numAtVal];
        }
    }
    public void SetHeroTwoNP(int hp)
    {
        string s = ConvertIntToStringOfLength(hp, heroTwoNPDigits.Length);
        for (int i = 0; i < heroTwoNPDigits.Length; i++)
        {
            int.TryParse(s.Substring(i, 1), out int numAtVal);
            heroTwoNPDigits[i].sprite = digits[numAtVal];
        }
    }

    private string ConvertIntToStringOfLength(int num, int length)
    {
        string newStr = "";
        for (int i = 0; i < length - num.ToString().Length; i++)
        {
            newStr += "0";
        }
        newStr += num.ToString();
        return newStr;
    }
}
