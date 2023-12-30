using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    private PlayerControls controls;
    private PlayerInput input;
    [SerializeField] private Sprite[] keyboardButtons;
    [SerializeField] private TMP_SpriteAsset keyboardSpriteAsset;
    [SerializeField] private Sprite[] xboxButtons;
    [SerializeField] private TMP_SpriteAsset xboxSpriteAsset;
    [SerializeField] private Sprite[] playstationButtons;
    [SerializeField] private TMP_SpriteAsset playstationSpriteAsset;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        controls = new PlayerControls();
        input = GetComponent<PlayerInput>();
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
    //*****************//
    //*BATTLE CONTROLS*//
    //*****************//

    // Menu
    public Vector2 GetMenuNavigation()
    {
        return controls.Menu.Navigate.ReadValue<Vector2>();
    }
    public bool GetConfirm()
    {
        return controls.Menu.Confirm.triggered;
    }
    public bool GetCancel()
    {
        return controls.Menu.Cancel.triggered;
    }
    public bool GetSwapHero()
    {
        return controls.Menu.SwapHero.triggered;
    }
    // Minigame
    public bool GetMinigameButtonAPressed()
    {
        return controls.Minigame.MinigameButtonA.triggered;
    }
    public bool GetMinigameButtonAHeld()
    {
        return controls.Minigame.MinigameButtonA.IsPressed();
    }
    public bool GetMinigameButtonBPressed()
    {
        return controls.Minigame.MinigameButtonB.triggered;
    }
    public bool GetMinigameButtonBHeld()
    {
        return controls.Minigame.MinigameButtonB.IsPressed();
    }
    public bool GetMinigameButtonXPressed()
    {
        return controls.Minigame.MinigameButtonX.triggered;
    }
    public bool GetMinigameButtonXHeld()
    {
        return controls.Minigame.MinigameButtonX.IsPressed();
    }
    public bool GetMinigameButtonYPressed()
    {
        return controls.Minigame.MinigameButtonY.triggered;
    }
    public bool GetMinigameButtonYHeld()
    {
        return controls.Minigame.MinigameButtonY.IsPressed();
    }
    public bool GetMinigameButtonUpPressed()
    {
        return controls.Minigame.MinigameDirectionalUp.triggered;
    }
    public bool GetMinigameButtonUpHeld()
    {
        return controls.Minigame.MinigameDirectionalUp.IsPressed();
    }
    public bool GetMinigameButtonDownPressed()
    {
        return controls.Minigame.MinigameDirectionalDown.triggered;
    }
    public bool GetMinigameButtonDownHeld()
    {
        return controls.Minigame.MinigameDirectionalDown.IsPressed();
    }
    public bool GetMinigameButtonLeftPressed()
    {
        return controls.Minigame.MinigameDirectionalLeft.triggered;
    }
    public bool GetMinigameButtonLeftHeld()
    {
        return controls.Minigame.MinigameDirectionalLeft.IsPressed();
    }
    public bool GetMinigameButtonRightPressed()
    {
        return controls.Minigame.MinigameDirectionalRight.triggered;
    }
    public bool GetMinigameButtonRightHeld()
    {
        return controls.Minigame.MinigameDirectionalRight.IsPressed();
    }
    // Enemy Phase
    public bool GetBlock()
    {
        return controls.Enemy.Block.triggered;
    }

    //******************//
    //*CONSOLE CONTROLS*//
    //******************//
    public bool GetConsoleToggled()
    {
        return controls.Console.ToggleConsole.triggered;
    }
    public bool GetConsoleSubmit()
    {
        return controls.Console.Submit.triggered;
    }
    //********************//
    //*OVERWORLD CONTROLS*//
    //********************//
    public Vector2 GetOverworldMovement()
    {
        return controls.Overworld.Movement.ReadValue<Vector2>();
    }
    public bool GetOverworldJump()
    {
        return controls.Overworld.Jump.triggered;
    }
    public bool GetOverworldInteract()
    {
        return controls.Overworld.Interact.triggered;
    }
    public bool GetOverworldSprint()
    {
        return controls.Overworld.Sprint.IsPressed();
    }

    //***************//
    //**INPUT TYPES**//
    //***************//
    public string GetControlScheme()
    {
        return input.currentControlScheme;
    }
    public Sprite[] GetInputSprites()
    {
        switch (input.currentControlScheme)
        {
            case "Keyboard":
                return keyboardButtons;
            case "Xbox":
                return xboxButtons;
            case "Playstation":
                return playstationButtons;
            default:
                Debug.LogError("ERROR: Could not find current control scheme!");
                return null;
        }
    }
    public TMP_SpriteAsset GetTextSpriteSheet()
    {
        switch (input.currentControlScheme)
        {
            case "Keyboard":
                return keyboardSpriteAsset;
            case "Xbox":
                return xboxSpriteAsset;
            case "Playstation":
                return playstationSpriteAsset;
            default:
                Debug.LogError("ERROR: Could not find current control scheme!");
                return null;
        }
    }
}
