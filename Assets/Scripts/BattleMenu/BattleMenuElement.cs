using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMenuElement : MonoBehaviour
{
    [SerializeField] protected BattleMenuElement elementOnUp;
    [SerializeField] protected BattleMenuElement elementOnDown;
    [SerializeField] protected BattleMenuElement elementOnLeft;
    [SerializeField] protected BattleMenuElement elementOnRight;
    public BattleMenuElement elementOnConfirm; // Public, since this is dynamic when in submenus
    public BattleMenuElement elementOnCancel; // Public, since this is dynamic when in submenus

    public bool validSelection = true;

    [Header("UI Visibility When Selected")]
    [SerializeField] private bool VIS_MAIN;
    [SerializeField] private bool VIS_SUBMENU_ABILITY;
    [SerializeField] private bool VIS_SUBMENU_ITEM;
    [SerializeField] private bool VIS_SUBMENU_OTHER;

    protected Animator animator;
    protected BattleMenuManager bmManager;
    protected BattleManager battleManager;
    protected void Awake()
    {
        animator = GetComponent<Animator>();
        bmManager = FindObjectOfType<BattleMenuManager>();
        battleManager = FindObjectOfType<BattleManager>();
    }
    public BattleMenuElement Navigate(Vector2 dir, int numIterationsTried = 0)
    {
        if(dir.x == -1 && elementOnLeft != null)
        {
            OnDeselect();
            if (elementOnLeft.OnSelected())
            {
                // Passed selection successfully
                return elementOnLeft;
            }
            else
            {
                // The sytem can try to keep going ten times before I'm cutting it off. This prevents a crash.
                if(numIterationsTried > 10)
                {
                    Debug.LogError("WARNING: BattleMenuElement iterated through Navigate() 10 times, and cannot find a target.");
                    throw new Exception();
                }
                // Need to reach further to get the next available element in this direction.
                BattleMenuElement nextElemInLine = elementOnLeft.Navigate(dir, numIterationsTried + 1);
                return nextElemInLine;
            }
        }
        else if (dir.x == 1 && elementOnRight != null)
        {
            OnDeselect();
            if (elementOnRight.OnSelected())
            {
                // Passed selection successfully
                return elementOnRight;
            }
            else
            {
                if (numIterationsTried > 10)
                {
                    Debug.LogError("WARNING: BattleMenuElement iterated through Navigate() 10 times, and cannot find a target.");
                    throw new Exception();
                }
                // Need to reach further to get the next available element in this direction.
                BattleMenuElement nextElemInLine = elementOnRight.Navigate(dir, numIterationsTried + 1);
                return nextElemInLine;
            }
        }
        else if (dir.y == 1 && elementOnUp != null)
        {
            OnDeselect();
            if (elementOnUp.OnSelected())
            {
                // Passed selection successfully
                return elementOnUp;
            }
            else
            {
                if (numIterationsTried > 10)
                {
                    Debug.LogError("WARNING: BattleMenuElement iterated through Navigate() 10 times, and cannot find a target.");
                    throw new Exception();
                }
                // Need to reach further to get the next available element in this direction.
                BattleMenuElement nextElemInLine = elementOnUp.Navigate(dir, numIterationsTried + 1);
                return nextElemInLine;
            }
        }
        else if (dir.y == -1 && elementOnDown != null)
        {
            OnDeselect();
            if (elementOnDown.OnSelected())
            {
                // Passed selection successfully
                return elementOnDown;
            }
            else
            {
                if (numIterationsTried > 10)
                {
                    Debug.LogError("WARNING: BattleMenuElement iterated through Navigate() 10 times, and cannot find a target.");
                    throw new Exception();
                }
                // Need to reach further to get the next available element in this direction.
                BattleMenuElement nextElemInLine = elementOnDown.Navigate(dir, numIterationsTried + 1);
                return nextElemInLine;
            }
        }
        return this;
    }
    public BattleMenuElement OnConfirm()
    {
        if (elementOnConfirm != null)
        {
            OnDeselect();

            if (elementOnConfirm.OnSelected())
            {
                // Passed selection successfully
                return elementOnConfirm;
            }
            else
            {

                // Need to reach further to get the next available element in this direction.
                // Default to looking further right, since this is desired behaviour for enemy selection
                BattleMenuElement nextElemInLine = elementOnConfirm.Navigate(Vector2.right);
                return nextElemInLine;
            }
        }
        return this;
    }
    public BattleMenuElement OnCancel()
    {
        if (elementOnCancel != null)
        {
            OnDeselect();
            elementOnCancel.OnSelected();
            return elementOnCancel;
        }
        return this;
    }
    public bool OnSelected()
    {
        // returns based on this is valid or not. If it's not, it'll send back to its selector and try to pass right from this one.
        if (validSelection)
        {
            animator.SetBool("Selected", true);
            bmManager.SetUIVisibility(VIS_MAIN, VIS_SUBMENU_ABILITY, VIS_SUBMENU_ITEM, VIS_SUBMENU_OTHER);
            return true;
        }
        else
        {
            print("Passing selection");
            return false;
        }
    }
    public void OnDeselect()
    {
        animator.SetBool("Selected", false);
    }
}
