using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMenuElement_DoAction : BattleMenuElement
{
    // This script can handle the Fight, NP Actions, and Items. It will pass on 
    [SerializeField] private HeroAction action;

    [Header("Selection")]    
    public bool canBeSelected = true;
    [SerializeField] private Image bgImage;
    [SerializeField] private Image npImage;

    [Header("Behaviour States")]
    [SerializeField] private bool needsHeroSelection;
    [SerializeField] private bool needsEnemySelection;

    [Header("Elements (First is Element On Confirm)")]
    [SerializeField] private BattleMenuElement_HeroSelector[] heroElements;
    [SerializeField] private BattleMenuElement_EnemySelector[] enemyElements;
    private readonly Color COLOR_ACTIVE = new(1, 1, 1);
    private readonly Color COLOR_INACTIVE = new(0.5f, 0.5f, 0.5f);
    public new BattleMenuElement OnConfirm()
    {
        if(!canBeSelected)
        {
            soundManager.PlaySound(sndInvalid);
            return this;
        }
        BattleMenuElement firstElem;
        if (elementOnConfirm == null)
        {
            battleManager.RelayActionToHero(action);
            soundManager.PlaySound(sndConfirm);
            return null;
        }
        else
        {
            OnDeselect();

            if (elementOnConfirm.OnSelected())
            {
                // Passed selection successfully
                soundManager.PlaySound(sndConfirm);
                firstElem = elementOnConfirm;
            }
            else
            {
                // Need to reach further to get the next available element in this direction.
                // Default to looking further right, since this is desired behaviour for enemy selection
                firstElem = elementOnConfirm.Navigate(Vector2.right);
            }
            if (firstElem != null)
            {
                if (needsHeroSelection)
                {
                    foreach (var heroElem in heroElements)
                    {
                        // Set the purpose of each enemy, and also tell it to return here.
                        heroElem.SetPurpose(action);
                        heroElem.SetElementOnCancel(this);
                    }
                    return firstElem;
                }
                else // Implicitly, this therefore must mean that needsEnemySelection is true.
                {
                    foreach (var enemyElem in enemyElements)
                    {
                        // Set the purpose of each enemy, and also tell it to return here.
                        enemyElem.SetPurpose(action);
                        enemyElem.SetElementOnCancel(this);
                    }
                    return firstElem;
                }
            }
            else
            {
                return this;
            }
        }
    }
    // This will always be called before any action is taken, so it is safe to assume the correct behaviour will be done.
    public void SetConfirmBehaviour(bool heroSelection, bool enemySelection)
    {
        needsHeroSelection = heroSelection;
        needsEnemySelection = enemySelection;
        if (needsHeroSelection)
        {
            elementOnConfirm = heroElements[0];
        }
        else if (needsEnemySelection)
        {
            // We actually want to pick the second element of the array since it corresponds to the leftmost enemy.
            elementOnConfirm = enemyElements[1];
        }
        else
        {
            elementOnConfirm = null;
        }
    }
    public void SetSelectable(bool selectable)
    {
        if(selectable)
        {
            canBeSelected = true;
            bgImage.color = COLOR_ACTIVE;
            npImage.color = COLOR_ACTIVE;
        }
        else
        {
            canBeSelected = false;
            bgImage.color = COLOR_INACTIVE;
            npImage.color = COLOR_INACTIVE;
        }
    }
}
