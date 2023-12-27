using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class CharacterSelectMenuElement_HowToPlay_Popup : MenuElement
{

    [SerializeField] private CanvasGroup group;
    [SerializeField] private TextMeshProUGUI text;
    private InputManager inputManager;
    private void Start()
    {
        inputManager = InputManager.Instance;
    }
    public override MenuElement Navigate(Vector2 dir, int numIterationsTried = 0)
    {
        return BaseNavigate(dir, numIterationsTried);
    }

    public override MenuElement OnConfirm()
    {
        return BaseOnConfirm();
    }

    public override MenuElement OnCancel()
    {
        return this;
    }

    public override bool OnSelected()
    {
        if (BaseOnSelected())
        {
            group.alpha = 1;
            text.spriteAsset = inputManager.GetTextSpriteSheet();
            return true;
        }
        return false;
    }

    public override void OnDeselect()
    {
        BaseOnDeselect();
        group.alpha = 0;
    }
}
