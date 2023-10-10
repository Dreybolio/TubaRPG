using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private BattleData firstScene;

    private InputManager inputManager;
    private LevelManager levelManager;
    private bool _buttonPressed = false;
    private void Start()
    {
        inputManager = InputManager.Instance;
        levelManager = LevelManager.Instance;
    }
    private void Update()
    {
        if (!_buttonPressed && inputManager.GetConfirm())
        {
            _buttonPressed = true;
            print(firstScene.sceneName);
            levelManager.LoadBattle(firstScene);
        }
    }
}
