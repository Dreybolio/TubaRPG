using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMenuElement_HeroSelector : BattleMenuElement
{
    private HeroAction selectionPurpose;
    public int targetIndex; // Can be 0 or 1
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
        soundManager.PlaySound(sndConfirm);
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
