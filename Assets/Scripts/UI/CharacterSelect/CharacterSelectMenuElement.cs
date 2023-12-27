using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterSelectButtonStatus
{
    SELECTED_AS_FIRST,
    SELECTED_AS_SECOND,
    UNSELECTED
}
public class CharacterSelectMenuElement : MenuElement
{
    [SerializeField] private Image statusSprite;
    [SerializeField] private Sprite sprSelectedAsFirst;
    [SerializeField] private Sprite sprSelectedAsSecond;
    [SerializeField] private HeroType buttonType;
    private CharacterSelectButtonStatus status = CharacterSelectButtonStatus.UNSELECTED;

    [SerializeField] private CharacterSelectController characterSelect;

    public override MenuElement Navigate(Vector2 dir, int numIterationsTried = 0)
    {
        return BaseNavigate(dir, numIterationsTried);
    }

    public override MenuElement OnConfirm()
    {
        soundManager.PlaySound(sndConfirm);
        characterSelect.OnButtonPressed(buttonType);
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
    public void SetStatus(CharacterSelectButtonStatus status)
    {
        this.status = status;
        // This changes the graphics of the button
        if(status == CharacterSelectButtonStatus.SELECTED_AS_FIRST)
        {
            statusSprite.enabled = true;
            statusSprite.sprite = sprSelectedAsFirst;
        }
        else if(status == CharacterSelectButtonStatus.SELECTED_AS_SECOND)
        {
            statusSprite.enabled = true;
            statusSprite.sprite = sprSelectedAsSecond;
        }
        else if (status == CharacterSelectButtonStatus.UNSELECTED)
        {
            statusSprite.enabled = false;
        }
        else
        {
            throw new System.Exception("Could not parse CharacterSelectButtonStatus");
        }
    }
    public CharacterSelectButtonStatus GetStatus()
    {
        return status;
    }
}
