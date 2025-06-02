using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMenuElement : MenuElement
{
    [Header("Text - Leave Blank for Empty")]
    [SerializeField] protected string descriptionBoxTextOnSelected = string.Empty;
    [SerializeField] protected string enemyNameBoxTextOnSelected = string.Empty;

    [Header("UI Visibility When Selected")]
    [SerializeField] private bool VIS_MAIN;
    [SerializeField] private bool VIS_SUBMENU_ABILITY;
    [SerializeField] private bool VIS_SUBMENU_ITEM;
    [SerializeField] private bool VIS_SUBMENU_OTHER;
    [SerializeField] private bool VIS_DESCBOX;
    [SerializeField] private bool VIS_ENBOX;

    protected BattleMenuManager bmManager;
    protected BattleManager battleManager;
    protected new void Awake()
    {
        base.Awake();
        bmManager = FindObjectOfType<BattleMenuManager>();
        battleManager = FindObjectOfType<BattleManager>();
    }
    public override MenuElement Navigate(Vector2 dir, int numIterationsTried = 0)
    {
        return BaseNavigate(dir, numIterationsTried);
    }
    public override MenuElement OnConfirm()
    {
        return BaseOnConfirm();
    }
    public override MenuElement OnCancel()
    {
        return BaseOnCancel();
    }
    public override bool OnSelected()
    {
        bool baseStatus = BaseOnSelected();
        // returns based on this is valid or not. If it's not, it'll send back to its selector and try to pass right from this one.
        if (baseStatus)
        {
            bmManager.SetUIVisibility(VIS_MAIN, VIS_SUBMENU_ABILITY, VIS_SUBMENU_ITEM, VIS_SUBMENU_OTHER, VIS_DESCBOX, VIS_ENBOX);
            if(descriptionBoxTextOnSelected != string.Empty)
            {
                bmManager.SetDescriptionBoxText(descriptionBoxTextOnSelected);
            }
            if(enemyNameBoxTextOnSelected!= string.Empty)
            {
                bmManager.SetEnemyNameBoxText(enemyNameBoxTextOnSelected);
            }
        }
        return baseStatus;
    }
    public override void OnDeselect()
    {
        BaseOnDeselect();
    }
    public void SetDescriptionBoxTextOnSelected(string description)
    {
        descriptionBoxTextOnSelected = description;
    }
    public void SetEnemyNameBoxTextOnSelected(string description)
    {
        enemyNameBoxTextOnSelected = description;
    }
}
