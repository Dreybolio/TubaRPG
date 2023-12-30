using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorListener : MonoBehaviour
{
    public delegate void AnimationEvent();
    public event AnimationEvent OnEvent01;
    public event AnimationEvent OnEvent02;
    public event AnimationEvent OnEvent03;
    public event AnimationEvent OnEvent04;
    public event AnimationEvent OnEvent05;
    public event AnimationEvent OnEvent06;
    public event AnimationEvent OnEvent07;
    public event AnimationEvent OnEvent08;

    public void TriggerEvent01()
    {
        OnEvent01?.Invoke();
    }
    public void TriggerEvent02()
    {
        OnEvent02?.Invoke();
    }
    public void TriggerEvent03()
    {
        OnEvent03?.Invoke();
    }
    public void TriggerEvent04()
    {
        OnEvent04?.Invoke();
    }
    public void TriggerEvent05()
    {
        OnEvent05?.Invoke();
    }
    public void TriggerEvent06()
    {
        OnEvent06?.Invoke();
    }
    public void TriggerEvent07()
    {
        OnEvent07?.Invoke();
    }
    public void TriggerEvent08()
    {
        OnEvent08?.Invoke();
    }
}
