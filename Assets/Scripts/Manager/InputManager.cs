using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;

    public static InputManager Instance
    {
        get { return instance; }
    }
    private BattleControls battleControls;
    private PlayerInput input;
    [SerializeField] private Sprite[] keyboardButtons;
    [SerializeField] private TMP_SpriteAsset keyboardSpriteAsset;
    [SerializeField] private Sprite[] xboxButtons;
    [SerializeField] private TMP_SpriteAsset xboxSpriteAsset;
    [SerializeField] private Sprite[] playstationButtons;
    [SerializeField] private TMP_SpriteAsset playstationSpriteAsset;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        battleControls = new BattleControls();
        input = GetComponent<PlayerInput>();
    }
    private void OnEnable()
    {
        battleControls.Enable();
    }
    private void OnDisable()
    {
        //battleControls.Disable();
    }
    //*****************//
    //*BATTLE CONTROLS*//
    //*****************//

    // Menu
    public Vector2 GetMenuNavigation()
    {
        return battleControls.Menu.Navigate.ReadValue<Vector2>();
    }
    public bool GetConfirm()
    {
        return battleControls.Menu.Confirm.triggered;
    }
    public bool GetCancel()
    {
        return battleControls.Menu.Cancel.triggered;
    }
    public bool GetSwapHero()
    {
        return battleControls.Menu.SwapHero.triggered;
    }
    // Minigame
    public bool GetMinigameButtonAPressed()
    {
        return battleControls.Minigame.MinigameButtonA.triggered;
    }
    public bool GetMinigameButtonAHeld()
    {
        return battleControls.Minigame.MinigameButtonA.IsPressed();
    }
    public bool GetMinigameButtonBPressed()
    {
        return battleControls.Minigame.MinigameButtonB.triggered;
    }
    public bool GetMinigameButtonBHeld()
    {
        return battleControls.Minigame.MinigameButtonB.IsPressed();
    }
    public bool GetMinigameButtonXPressed()
    {
        return battleControls.Minigame.MinigameButtonX.triggered;
    }
    public bool GetMinigameButtonXHeld()
    {
        return battleControls.Minigame.MinigameButtonX.IsPressed();
    }
    public bool GetMinigameButtonYPressed()
    {
        return battleControls.Minigame.MinigameButtonY.triggered;
    }
    public bool GetMinigameButtonYHeld()
    {
        return battleControls.Minigame.MinigameButtonY.IsPressed();
    }
    public bool GetMinigameButtonUpPressed()
    {
        return battleControls.Minigame.MinigameDirectionalUp.triggered;
    }
    public bool GetMinigameButtonUpHeld()
    {
        return battleControls.Minigame.MinigameDirectionalUp.IsPressed();
    }
    public bool GetMinigameButtonDownPressed()
    {
        return battleControls.Minigame.MinigameDirectionalDown.triggered;
    }
    public bool GetMinigameButtonDownHeld()
    {
        return battleControls.Minigame.MinigameDirectionalDown.IsPressed();
    }
    public bool GetMinigameButtonLeftPressed()
    {
        return battleControls.Minigame.MinigameDirectionalLeft.triggered;
    }
    public bool GetMinigameButtonLeftHeld()
    {
        return battleControls.Minigame.MinigameDirectionalLeft.IsPressed();
    }
    public bool GetMinigameButtonRightPressed()
    {
        return battleControls.Minigame.MinigameDirectionalRight.triggered;
    }
    public bool GetMinigameButtonRightHeld()
    {
        return battleControls.Minigame.MinigameDirectionalRight.IsPressed();
    }
    // Enemy Phase
    public bool GetBlock()
    {
        return battleControls.Enemy.Block.triggered;
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
