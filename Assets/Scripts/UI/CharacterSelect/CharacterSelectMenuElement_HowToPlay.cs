using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CharacterSelectMenuElement_HowToPlay : MenuElement
{

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
