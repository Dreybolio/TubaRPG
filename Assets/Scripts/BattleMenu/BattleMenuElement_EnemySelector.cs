using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BattleMenuElement_EnemySelector : BattleMenuElement
{
    private HeroAction selectionPurpose;
    public int targetIndex; // Can be 0, 1, or 2
    protected new void Awake()
    {
        base.Awake();
    }
    public new BattleMenuElement OnConfirm()
    {
        // Tell the battlemanager that its current hero shall
        // undergo the selection purpose action against 
        // the target associated with this selector.
        battleManager.RelayActionToHero(selectionPurpose, targetIndex);
        return null;
    }
    public void SetPurpose(HeroAction purpose)
    {
        selectionPurpose = purpose;
    }
    public void SetElementOnCancel(BattleMenuElement elem)
    {
        elementOnCancel = elem;
    }
}
