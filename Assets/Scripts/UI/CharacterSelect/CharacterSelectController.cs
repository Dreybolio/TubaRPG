using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectController : MonoBehaviour
{
    [SerializeField] private BattleData firstScene;

    [SerializeField] private CharacterSelectMenuElement barbarianButton;
    [SerializeField] private CharacterSelectMenuElement rogueButton;
    [SerializeField] private CharacterSelectMenuElement druidButton;
    [SerializeField] private CharacterSelectMenuElement wizardButton;



    HeroType heroOne;
    HeroType heroTwo;

    int _firstSelectionCounter = 0;
    bool _selectionToggle = true;
    private bool _menuNavCooldown = true;
    private MenuElement _selectedMenuElement;

    private GameData gameData;
    private LevelManager levelManager;
    private InputManager inputManager;
    private void Start()
    {
        gameData = GameData.Instance;
        levelManager = LevelManager.Instance;
        inputManager = InputManager.Instance;

        _selectedMenuElement = barbarianButton;
        barbarianButton.OnSelected();
    }
    private void Update()
    {
        // Navigate if coming from zero navigation. Otherwise, make the player reset the button
        if (_menuNavCooldown && inputManager.GetMenuNavigation() != Vector2.zero)
        {
            Navigate(inputManager.GetMenuNavigation());
            _menuNavCooldown = false;
        }
        else if (inputManager.GetMenuNavigation() == Vector2.zero) { _menuNavCooldown = true; }

        // Confirm
        if (inputManager.GetConfirm()) { Confirm(); }
    }
    private void Navigate(Vector2 dir)
    {
        MenuElement newElem = _selectedMenuElement.Navigate(dir);
        if (newElem != null && newElem != _selectedMenuElement)
        {
            _selectedMenuElement = newElem;
        }
    }
    private void Confirm()
    {
        MenuElement newElem;

        // Iterate through all the things this element might be. Done so we can call the function of the subclass.
        if (_selectedMenuElement is CharacterSelectMenuElement menuElem) { newElem = menuElem.OnConfirm(); }
        else if (_selectedMenuElement is CharacterSelectMenuElement_Confirm confirmElem) { newElem = confirmElem.OnConfirm(); }
        else { newElem = _selectedMenuElement.OnConfirm(); }

        if (newElem != null && newElem != _selectedMenuElement)
        {
            _selectedMenuElement = newElem;
        }
    }
    /**
     *  When the UI Button is pressed, type is HeroType
     */
    public void OnButtonPressed(HeroType btnType)
    {
        // The first two selections must always be made before anything else
        if(_firstSelectionCounter < 2)
        {
            bool success = ProcessFirstSelections(btnType, _firstSelectionCounter);
            if(success)
            {
                // Only increment if successful
                _firstSelectionCounter++;
            }
            return;
        }
        // Made to to this point: Two heroes have already been selected, and we're selecting again
        if(btnType != heroOne && btnType != heroTwo)
        {
            // Pressing a new, third hero
            if(_selectionToggle)
            {
                // Toggle: We're going to replace heroOne
                UpdateButtonUI(heroOne, CharacterSelectButtonStatus.UNSELECTED);
                heroOne = btnType;
                UpdateButtonUI(heroOne, CharacterSelectButtonStatus.SELECTED_AS_FIRST);
                _selectionToggle = !_selectionToggle;
            }
            else
            {
                // Toggle: We're going to replace heroTwo
                UpdateButtonUI(heroTwo, CharacterSelectButtonStatus.UNSELECTED);
                heroTwo = btnType;
                UpdateButtonUI(heroTwo, CharacterSelectButtonStatus.SELECTED_AS_SECOND);
                _selectionToggle = !_selectionToggle;
            }
        }
        else
        {
            // Pressed either heroOne or heroTwo
            // Therefore, we are going to swap heroOne and heroTwo
            HeroType storage;
            storage = heroOne;
            heroOne = heroTwo;
            heroTwo = storage;
            UpdateButtonUI(heroOne, CharacterSelectButtonStatus.SELECTED_AS_FIRST);
            UpdateButtonUI(heroTwo, CharacterSelectButtonStatus.SELECTED_AS_SECOND);
        }


    }
    /**
     *  When confirm is pressed
     */
    public void OnConfirmPressed()
    {
        if(_firstSelectionCounter >= 2)
        {
            // Only proceed if we've selected two heroes
            gameData.SetHeroOne(heroOne);
            gameData.SetHeroTwo(heroTwo);
            levelManager.LoadBattle(firstScene);
        }
    }
    /**
     * Returns a boolean to signify operation success
     */
    private bool ProcessFirstSelections(HeroType btnType, int count)
    {
        if(count == 0)
        {
            heroOne = btnType;
            UpdateButtonUI(btnType, CharacterSelectButtonStatus.SELECTED_AS_FIRST);
            return true;
        }
        else if(heroOne != btnType)
        {
            // This may only happen if the second selection is not equal to the first selection
            heroTwo = btnType;
            UpdateButtonUI(btnType, CharacterSelectButtonStatus.SELECTED_AS_SECOND);
            return true;
        }
        return false;
    }
    /**
     *  Updates the UI of the HeroType button to be the passsed status. May be SelectedFirst, SelectedSecond, or Unselected
     */
    private void UpdateButtonUI(HeroType btnType, CharacterSelectButtonStatus status)
    {
        switch (btnType)
        {
            case HeroType.BARBARIAN:
                barbarianButton.SetStatus(status);
                break;
            case HeroType.ROGUE:
                rogueButton.SetStatus(status);
                break;
            case HeroType.DRUID:
                druidButton.SetStatus(status);
                break;
            case HeroType.WIZARD:
                wizardButton.SetStatus(status);
                break;
        }
    }
}
