using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HeroType
{
    BARBARIAN,
    ROGUE,
    DRUID,
    WIZARD
}
public class GameData : MonoBehaviour
{
    // Only allow one instance of
    #region 
    private static GameData instance;

    public static GameData Instance
    {
        get { return instance; }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    // Public, used data
    [NonSerialized] public int heroOne_Level;
    [NonSerialized] public int heroOne_HPAllocations;
    [NonSerialized] public int heroOne_NPAllocations;
    [NonSerialized] public HeroType heroOne_HeroType;
    [NonSerialized] public GameObject heroOne_Object;
    [NonSerialized] public Sprite heroOne_Icon;

    [NonSerialized] public int heroTwo_Level;
    [NonSerialized] public int heroTwo_HPAllocations;
    [NonSerialized] public int heroTwo_NPAllocations;
    [NonSerialized] public HeroType heroTwo_HeroType;
    [NonSerialized] public GameObject heroTwo_Object;
    [NonSerialized] public Sprite heroTwo_Icon;

    [NonSerialized] public Item[] items;

    // Data for assignment
    [SerializeField] private GameObject barbarianObject;
    [SerializeField] private GameObject rogueObject;
    [SerializeField] private GameObject druidObject;
    [SerializeField] private GameObject wizardObject;
    [SerializeField] private Sprite barbarianIcon;
    [SerializeField] private Sprite rogueIcon;
    [SerializeField] private Sprite druidIcon;
    [SerializeField] private Sprite wizardIcon;
    public void SetHeroOne(HeroType heroType)
    {
        heroOne_HeroType = heroType;
        switch (heroType)
        {
            case HeroType.BARBARIAN:
                heroOne_Object = barbarianObject;
                heroOne_Icon = barbarianIcon;
                break;
            case HeroType.ROGUE:
                heroOne_Object = rogueObject;
                heroOne_Icon = rogueIcon;
                break;
            case HeroType.DRUID:
                heroOne_Object = druidObject;
                heroOne_Icon = druidIcon;
                break;
            case HeroType.WIZARD:
                heroOne_Object = wizardObject;
                heroOne_Icon = wizardIcon;
                break;
        }
    }
    public void SetHeroTwo(HeroType heroType)
    {
        heroTwo_HeroType = heroType;
        switch (heroType)
        {
            case HeroType.BARBARIAN:
                heroTwo_Object = barbarianObject;
                heroTwo_Icon = barbarianIcon;
                break;
            case HeroType.ROGUE:
                heroTwo_Object = rogueObject;
                heroTwo_Icon = rogueIcon;
                break;
            case HeroType.DRUID:
                heroTwo_Object = druidObject;
                heroTwo_Icon = druidIcon;
                break;
            case HeroType.WIZARD:
                heroTwo_Object = wizardObject;
                heroTwo_Icon = wizardIcon;
                break;
        }
    }
}
