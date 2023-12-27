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
    [SerializeField] private CanvasGroup groupDescriptionBox;
    [SerializeField] private CanvasGroup groupEnemyNameBox;


    [Header("Fight Menu")]
    [SerializeField] private BattleMenuElement_DoAction fightScript;

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
    [SerializeField] private HealthBar[] enemyHealthBars;

    [Header("Status Effect Builders")]
    [SerializeField] private BattleMenuStatusEffectHandler heroOneStatusEffectHandler;
    [SerializeField] private BattleMenuStatusEffectHandler heroTwoStatusEffectHandler;
    [SerializeField] private BattleMenuStatusEffectHandler enemyZeroStatusEffectHandler;
    [SerializeField] private BattleMenuStatusEffectHandler enemyOneStatusEffectHandler;
    [SerializeField] private BattleMenuStatusEffectHandler enemyTwoStatusEffectHandler;

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

    [Header("DescriptionBox")]
    [SerializeField] private DescriptionBox dBox;
    [SerializeField] private DescriptionBox enBox;

    [Header("Damage Indicator")]
    [SerializeField] private Transform damageIndicatorHolder;
    [SerializeField] private GameObject damageIndicatorPrefab;

    // Pointers
    private BattleManager battleManager;

    private void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();
    }
    public void SetUIVisibility(bool main, bool subAbility, bool subItem, bool subOther, bool descBox, bool enBox)
    {
        groupMain.alpha = main ? 1f : 0f;
        groupSubAbility.alpha = subAbility ? 1f : 0f;
        groupSubItem.alpha = subItem ? 1f : 0f;
        groupSubOther.alpha = subOther ? 1f : 0f;
        groupDescriptionBox.alpha = descBox ? 1f : 0f;
        groupEnemyNameBox.alpha = enBox ? 1f : 0f;
    }
    public void SetMenuData(GenericHero hero)
    {
        fightScript.SetDescriptionBoxTextOnSelected(hero.attack.description);

        abilityOneText.text = hero.abilityOne.name;
        abilityOneNPCost.text = hero.abilityOne.npCost.ToString();
        abilityOneScript.SetConfirmBehaviour(hero.abilityOne.requiresHeroSelection, hero.abilityOne.requiresEnemySelection);
        abilityOneScript.SetConfirmable(hero.GetNP() >= hero.abilityOne.npCost);
        abilityOneScript.SetDescriptionBoxTextOnSelected(hero.abilityOne.description);

        abilityTwoText.text = hero.abilityTwo.name;
        abilityTwoNPCost.text = hero.abilityTwo.npCost.ToString();
        abilityTwoScript.SetConfirmBehaviour(hero.abilityTwo.requiresHeroSelection, hero.abilityTwo.requiresEnemySelection);
        abilityTwoScript.SetConfirmable(hero.GetNP() >= hero.abilityTwo.npCost);
        abilityTwoScript.SetDescriptionBoxTextOnSelected(hero.abilityTwo.description);

        // Any manual exceptions are handled here

        // FOR BARBARIAN: If the other hero isn't alive, you cannot use Invigorating Tune
        if(hero.type == HeroType.BARBARIAN)
        {
            GenericHero otherHero = battleManager.GetHeroIndex(hero) == 0 ? battleManager.GetHero(1) : battleManager.GetHero(0);
            if (!otherHero.isAlive)
            {
                abilityTwoScript.SetConfirmable(false);
            }
        }

        // FOR ROGUE: If you already are under the effects of Decrescendo, you cannot use Decrescendo again
        if (hero.type == HeroType.ROGUE)
        {
            if (hero.statusEffects.ContainsKey(StatusEffect.DECRESCENDO))
            {
                abilityOneScript.SetConfirmable(false);
            }
        }
    }
    public void SetEnemyName(int index, string enemyName)
    {
        BattleMenuElement elem = null;
        if(index == 0) { elem = enemyZeroSelector; }
        else if(index == 1) {  elem = enemyOneSelector; }
        else if(index == 2) {  elem = enemyTwoSelector; }
        if(elem != null)
        {
            elem.SetEnemyNameBoxTextOnSelected(enemyName);
        }
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
    public void SetEnemyHPBarMaxValue(int index, int maxHP, bool enabled = true)
    {
        if (enabled)
        {
            enemyHealthBars[index].Enable();
        }
        else
        {
            enemyHealthBars[index].Disable();
        }
        enemyHealthBars[index].SetMaximum(maxHP, true);
    }
    public void SetEnemyHPBarValue(int index, int hp, bool enabled = true)
    {
        if (enabled)
        {
            enemyHealthBars[index].Enable();
        }
        else
        {
            enemyHealthBars[index].Disable();
        }
        enemyHealthBars[index].SetValue(hp);
    }
    public void SetHeroIcon(int index, Sprite spr)
    {
        if(index == 0)
        {
            heroOneIcon.sprite = spr;
        }else if(index == 1)
        {
            heroTwoIcon.sprite = spr;
        }
    }
    public void SetHeroMaxHP(int index, int maxHP)
    {
        if(index == 0)
        {
            string s = ConvertIntToStringOfLength(maxHP, heroOneMaxHPDigits.Length);
            for (int i = 0; i < heroOneMaxHPDigits.Length; i++)
            {
                int.TryParse(s.Substring(i, 1), out int numAtVal);
                heroOneMaxHPDigits[i].sprite = digits[numAtVal];
            }
        }else if(index == 1)
        {
            string s = ConvertIntToStringOfLength(maxHP, heroTwoMaxHPDigits.Length);
            for (int i = 0; i < heroTwoMaxHPDigits.Length; i++)
            {
                int.TryParse(s.Substring(i, 1), out int numAtVal);
                heroTwoMaxHPDigits[i].sprite = digits[numAtVal];
            }
        }
    }
    public void SetHeroHP(int index, int hp)
    {
        if(index == 0)
        {
            string s = ConvertIntToStringOfLength(hp, heroOneHPDigits.Length);
            for (int i = 0; i < heroOneHPDigits.Length; i++)
            {
                int.TryParse(s.Substring(i, 1), out int numAtVal);
                heroOneHPDigits[i].sprite = digits[numAtVal];
            }
        }
        else if(index == 1)
        {
            string s = ConvertIntToStringOfLength(hp, heroTwoHPDigits.Length);
            for (int i = 0; i < heroTwoHPDigits.Length; i++)
            {
                int.TryParse(s.Substring(i, 1), out int numAtVal);
                heroTwoHPDigits[i].sprite = digits[numAtVal];
            }
        }
    }
    public void SetHeroMaxNP(int index, int hp)
    {
        if(index == 0)
        {
            string s = ConvertIntToStringOfLength(hp, heroOneMaxNPDigits.Length);
            for (int i = 0; i < heroOneMaxNPDigits.Length; i++)
            {
                int.TryParse(s.Substring(i, 1), out int numAtVal);
                heroOneMaxNPDigits[i].sprite = digits[numAtVal];
            }
        }
        else if(index == 1) 
        {
            string s = ConvertIntToStringOfLength(hp, heroTwoMaxNPDigits.Length);
            for (int i = 0; i < heroTwoMaxNPDigits.Length; i++)
            {
                int.TryParse(s.Substring(i, 1), out int numAtVal);
                heroTwoMaxNPDigits[i].sprite = digits[numAtVal];
            }
        }
    }
    public void SetHeroNP(int index, int hp)
    {
        if(index == 0)
        {
            string s = ConvertIntToStringOfLength(hp, heroOneNPDigits.Length);
            for (int i = 0; i < heroOneNPDigits.Length; i++)
            {
                int.TryParse(s.Substring(i, 1), out int numAtVal);
                heroOneNPDigits[i].sprite = digits[numAtVal];
            }
        }else if(index == 1)
        {
            string s = ConvertIntToStringOfLength(hp, heroTwoNPDigits.Length);
            for (int i = 0; i < heroTwoNPDigits.Length; i++)
            {
                int.TryParse(s.Substring(i, 1), out int numAtVal);
                heroTwoNPDigits[i].sprite = digits[numAtVal];
            }
        }
    }
    public void SpawnDamageIndicator(Vector2 pos, DamageIndicatorType type, int num)
    {
        GameObject obj = Instantiate(damageIndicatorPrefab, damageIndicatorHolder);
        obj.GetComponent<RectTransform>().anchoredPosition = pos;
        obj.GetComponent<DamageIndicator>().SetData(type, num);
    }
    public void SetHeroStatusEffects(int index, Dictionary<StatusEffect, int> dict)
    {
        if (index == 0)
        {
            heroOneStatusEffectHandler.SetStatusEffects(dict);
        }
        else if(index == 1)
        {
            heroTwoStatusEffectHandler.SetStatusEffects(dict);
        }
    }
    public void SetEnemyStatusEffects(int index, Dictionary<StatusEffect, int> dict)
    {
        if (index == 0)
        {
            enemyZeroStatusEffectHandler.SetStatusEffects(dict);
        }
        else if(index == 1)
        {
            enemyOneStatusEffectHandler.SetStatusEffects(dict);
        }
        else if (index == 2)
        {
            enemyTwoStatusEffectHandler.SetStatusEffects(dict) ;
        }
    }
    public void SetDescriptionBoxText(string text)
    {
        dBox.SetText(text);
    }
    public void SetEnemyNameBoxText(string text)
    {
        enBox.SetText(text);
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
