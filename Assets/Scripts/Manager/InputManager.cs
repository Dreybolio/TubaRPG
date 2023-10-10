using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;

    public static InputManager Instance
    {
        get { return instance; }
    }
    private BattleControls battleControls;
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
    public bool GetMinigameButton1Pressed()
    {
        return battleControls.Minigame.MiniGameButton1.triggered;
    }
    public bool GetMinigameButton1Held()
    {
        return battleControls.Minigame.MiniGameButton1.IsPressed();
    }
    public bool GetMinigameButton2Pressed()
    {
        return battleControls.Minigame.MiniGameButton2.triggered;
    }
    public bool GetMinigameButton2Held()
    {
        return battleControls.Minigame.MiniGameButton2.IsPressed();
    }
    public bool GetMinigameButton3Pressed()
    {
        return battleControls.Minigame.MiniGameButton3.triggered;
    }
    public bool GetMinigameButton3Held()
    {
        return battleControls.Minigame.MiniGameButton3.IsPressed();
    }
    public bool GetMinigameButton4Pressed()
    {
        return battleControls.Minigame.MiniGameButton4.triggered;
    }
    public bool GetMinigameButton4Held()
    {
        return battleControls.Minigame.MiniGameButton4.IsPressed();
    }
    public bool GetMinigameButtonUpPressed()
    {
        return battleControls.Minigame.MiniGameDirectionalUp.triggered;
    }
    public bool GetMinigameButtonUpHeld()
    {
        return battleControls.Minigame.MiniGameDirectionalUp.IsPressed();
    }
    public bool GetMinigameButtonDownPressed()
    {
        return battleControls.Minigame.MiniGameDirectionalDown.triggered;
    }
    public bool GetMinigameButtonDownHeld()
    {
        return battleControls.Minigame.MiniGameDirectionalDown.IsPressed();
    }
    public bool GetMinigameButtonLeftPressed()
    {
        return battleControls.Minigame.MiniGameDirectionalLeft.triggered;
    }
    public bool GetMinigameButtonLeftHeld()
    {
        return battleControls.Minigame.MiniGameDirectionalLeft.IsPressed();
    }
    public bool GetMinigameButtonRight()
    {
        return battleControls.Minigame.MiniGameDirectionalRight.triggered;
    }
    public bool GetMinigameButtonRightHeld()
    {
        return battleControls.Minigame.MiniGameDirectionalRight.IsPressed();
    }
    // Enemy Phase
    public bool GetBlock()
    {
        return battleControls.Enemy.Block.triggered;
    }
}
