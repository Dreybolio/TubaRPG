using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class MenuElement : MonoBehaviour
{
    [Header("Navigation")]
    public MenuElement elementOnUp;
    public MenuElement elementOnDown;
    public MenuElement elementOnLeft;
    public MenuElement elementOnRight;
    public MenuElement elementOnConfirm;
    public MenuElement elementOnCancel;

    public bool validSelection = true;

    [Header("SFX")]
    [SerializeField] protected AudioClip sndNavigate;
    [SerializeField] protected AudioClip sndConfirm;
    [SerializeField] protected AudioClip sndCancel;
    [SerializeField] protected AudioClip sndInvalid;

    protected Animator animator;
    protected SoundManager soundManager;

    protected void Awake()
    {
        animator = GetComponent<Animator>();
        soundManager = SoundManager.Instance;
    }
    /**
     * Moves selector to the next element in line. Returns that element.
     * Abstract so subclasses can easily navigate, but this base method is always called from within.
     */
    public abstract MenuElement Navigate(Vector2 dir, int numIterationsTried = 0);
    protected MenuElement BaseNavigate(Vector2 dir, int numIterationsTried = 0)
    {
        if (dir.x == -1 && elementOnLeft != null)
        {
            OnDeselect();
            if (elementOnLeft.OnSelected())
            {
                // Passed selection successfully
                soundManager.PlaySound(sndNavigate);
                return elementOnLeft;
            }
            else
            {
                // The sytem can try to keep going ten times before I'm cutting it off. This prevents a crash.
                if (numIterationsTried > 10)
                {
                    Debug.LogError("WARNING: BattleMenuElement iterated through Navigate() 10 times, and cannot find a target.");
                    throw new Exception();
                }
                // Need to reach further to get the next available element in this direction.
                MenuElement nextElemInLine = elementOnLeft.Navigate(dir, numIterationsTried + 1);
                return nextElemInLine;
            }
        }
        else if (dir.x == 1 && elementOnRight != null)
        {
            OnDeselect();
            if (elementOnRight.OnSelected())
            {
                // Passed selection successfully
                soundManager.PlaySound(sndNavigate);
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
                MenuElement nextElemInLine = elementOnRight.Navigate(dir, numIterationsTried + 1);
                return nextElemInLine;
            }
        }
        else if (dir.y == 1 && elementOnUp != null)
        {
            OnDeselect();
            if (elementOnUp.OnSelected())
            {
                // Passed selection successfully
                soundManager.PlaySound(sndNavigate);
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
                MenuElement nextElemInLine = elementOnUp.Navigate(dir, numIterationsTried + 1);
                return nextElemInLine;
            }
        }
        else if (dir.y == -1 && elementOnDown != null)
        {
            OnDeselect();
            if (elementOnDown.OnSelected())
            {
                // Passed selection successfully
                soundManager.PlaySound(sndNavigate);
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
                MenuElement nextElemInLine = elementOnDown.Navigate(dir, numIterationsTried + 1);
                return nextElemInLine;
            }
        }
        return this;
    }
    /**
    * Moves selector to the elementOnConfirm. Returns that element.
    * Can be easily overwritten to provide more specific funcitonality
    * Abstract so subclasses can easily navigate, but this base method is always called from within.
    */
    public abstract MenuElement OnConfirm();
    protected MenuElement BaseOnConfirm()
    {
        if (!validSelection)
        {
            soundManager.PlaySound(sndInvalid);
            return this;
        }
        if (elementOnConfirm != null)
        {
            OnDeselect();

            if (elementOnConfirm.OnSelected())
            {
                // Passed selection successfully
                soundManager.PlaySound(sndConfirm);
                return elementOnConfirm;
            }
            else
            {

                // Need to reach further to get the next available element in this direction.
                // Default to looking further right, since this is desired behaviour for enemy selection
                MenuElement nextElemInLine = elementOnConfirm.Navigate(Vector2.right);
                return nextElemInLine;
            }
        }
        return this;
    }
    /**
    * Returns selector to elementOnCancel. Returns that element.
    * Abstract so subclasses can easily navigate, but this base method is always called from within.
    */
    public abstract MenuElement OnCancel();
    protected MenuElement BaseOnCancel()
    {
        if (elementOnCancel != null)
        {
            OnDeselect();
            elementOnCancel.OnSelected();
            soundManager.PlaySound(sndCancel);
            return elementOnCancel;
        }
        return this;
    }
    /**
     *  Method call for when this element is selected
     *  Abstract so subclasses can easily navigate, but this base method is always called from within.
     */
    public abstract bool OnSelected();
    protected bool BaseOnSelected()
    {
        if(validSelection)
        {
            animator.SetBool("Selected", true);
        }
        return validSelection;
    }
    /**
    *  Method call for when this element is deselected
    *  Abstract so subclasses can easily navigate, but this base method is always called from within.
    */
    public abstract void OnDeselect();
    protected void BaseOnDeselect()
    {
        animator.SetBool("Selected", false);
    }
}
