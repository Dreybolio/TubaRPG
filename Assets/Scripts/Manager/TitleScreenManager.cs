using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pressAText;
    private InputManager inputManager;
    private LevelManager levelManager;

    private bool _buttonPressed = false;
    
    private void Start()
    {
        inputManager = InputManager.Instance;
        levelManager = LevelManager.Instance;
        // TODO: Fix this when building the real mainmenu
        // Currently throws exception since no input has been delivered yet.
        //pressAText.spriteAsset = inputManager.GetTextSpriteSheet();
    }
    private void Update()
    {
        if (!_buttonPressed && inputManager.GetConfirm())
        {
            _buttonPressed = true;
            levelManager.LoadScene("CharacterSelect");
        }
    }
}
