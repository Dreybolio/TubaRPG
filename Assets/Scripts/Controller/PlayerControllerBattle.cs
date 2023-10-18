using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlType
{
    None,
    Menu,
    Minigame,
    Blocking
}
public class PlayerControllerBattle : MonoBehaviour
{
    // Data
    private ControlType controlType;

    // Pointers
    private InputManager inputManager;
    private BattleManager battleManager;

    // Vars
    private bool menuNavCooldown = true;
    private BattleMenuElement selectedMenuElement;

    private void Start()
    {
        controlType = ControlType.None;
        inputManager = InputManager.Instance;
        battleManager = FindObjectOfType<BattleManager>();
    }
    public void SetControlType(ControlType ct)
    {
        controlType = ct;
        if(controlType == ControlType.Menu)
        {
            // Set default element to the fight button
            // This is inefficient... Too bad!
            selectedMenuElement = GameObject.Find("BMElem_Fight").GetComponent<BattleMenuElement>();
            selectedMenuElement.OnSelected();
        }
        else if (controlType == ControlType.None)
        {
            if(selectedMenuElement != null) { selectedMenuElement.OnDeselect(); }
            selectedMenuElement = null;
        }
    }
    private void Update()
    {
        switch (controlType)
        {
            case ControlType.None:
                break;
            case ControlType.Menu:
                // Navigate if coming from zero navigation. Otherwise, make the player reset the button
                if (menuNavCooldown && inputManager.GetMenuNavigation() != Vector2.zero)
                {
                    Navigate(inputManager.GetMenuNavigation());
                    menuNavCooldown = false;
                }
                else if (inputManager.GetMenuNavigation() == Vector2.zero) { menuNavCooldown = true; }

                // Confirm or cancel
                if (inputManager.GetConfirm()) { Confirm(); } 
                else if (inputManager.GetCancel()) { Cancel(); }
                else if (inputManager.GetSwapHero()) { SwapHero(); }

                break;
            case ControlType.Minigame:
                // Minigames ask for their own input
            case ControlType.Blocking:
                if(inputManager.GetBlock()) { Block(); }

                break;
        }
    }
    private void Navigate(Vector2 dir)
    {
        BattleMenuElement newElem = selectedMenuElement.Navigate(dir);
        if (newElem != null && newElem != selectedMenuElement) 
        {
            selectedMenuElement = newElem;
        }
    }
    private void Confirm()
    {
        BattleMenuElement newElem;

        // Iterate through all the things this element might be. Done so we can call the function of the superclass.
        if(selectedMenuElement is BattleMenuElement_DoAction actionElem) { newElem = actionElem.OnConfirm(); }
        else if (selectedMenuElement is BattleMenuElement_EnemySelector enemyElem) { newElem = enemyElem.OnConfirm(); }
        else { newElem = selectedMenuElement.OnConfirm(); }

        if (newElem != null && newElem != selectedMenuElement)
        {
            selectedMenuElement = newElem;
        }
    }
    private void Cancel()
    {
        BattleMenuElement newElem = selectedMenuElement.OnCancel();
        if (newElem != null && newElem != selectedMenuElement)
        {
            selectedMenuElement = newElem;
        }
    }
    private void SwapHero()
    {
        battleManager.SwapHero();
    }
    private void Block()
    {
        battleManager.OnHeroBlocked();
    }
}
