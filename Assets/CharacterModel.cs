using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class CharacterModel : MonoBehaviour
{
    [SerializeField] private RuntimeAnimatorController battleController;
    [SerializeField] private RuntimeAnimatorController overworldController;
    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void SetAsBattleModel()
    {
        animator.runtimeAnimatorController = battleController;
    }
    public void SetAsOverworldModel()
    {
        animator.runtimeAnimatorController = overworldController;
    }
}
