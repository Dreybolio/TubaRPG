using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CharacterSelectMenuElement_Confirm : MenuElement
{
    [SerializeField] private CharacterSelectController characterSelect;

    public override MenuElement Navigate(Vector2 dir, int numIterationsTried = 0)
    {
        return BaseNavigate(dir, numIterationsTried);
    }

    public override MenuElement OnConfirm()
    {
        soundManager.PlaySound(sndConfirm);
        characterSelect.OnConfirmPressed();
        return this;
    }

    public override MenuElement OnCancel()
    {
        return this;
    }

    public override bool OnSelected()
    {
        return BaseOnSelected();
    }

    public override void OnDeselect()
    {
        BaseOnDeselect();
    }
}
