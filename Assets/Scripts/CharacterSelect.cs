using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private BattleData firstScene;

    [SerializeField] private CharacterSelectButton barbarianButton;
    [SerializeField] private CharacterSelectButton rogueButton;
    [SerializeField] private CharacterSelectButton druidButton;
    [SerializeField] private CharacterSelectButton wizardButton;

    HeroType heroOne;
    HeroType heroTwo;

    int _firstSelectionCounter = 0;
    bool _selectionToggle = true;

    private GameData gameData;
    private LevelManager levelManager;
    private void Start()
    {
        gameData = GameData.Instance;
        levelManager = LevelManager.Instance;
    }
    /**
     *  When the UI Button is pressed, type is HeroType
     */
    public void OnButtonPressed(string type)
    {
        if(!Enum.TryParse(type, out HeroType btnType))
        {
            throw new Exception("Could not parse HeroType from string");
        }
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
