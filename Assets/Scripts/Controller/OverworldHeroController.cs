using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.InputSystem;

public class OverworldHeroController : OverworldHero
{
    [Header("Positional Data")]
    public Transform followPoint;
    [SerializeField] private float followPointDistance = 1.25f;
    // Pointers
    private InputManager inputManager;
    private new void Start()
    {
        base.Start();
        inputManager = InputManager.Instance;
    }
    private void Update()
    {
        if (!_modelIsValid)
        {
            Debug.LogError("Warning: HeroFollower has no model or animator!");
            return;
        }
        OverrideJumpAndGravity();
        GroundedCheck();
        OverrideMove();
        CheckForTurnAround();
        ApplyVelocity();
    }
    private void OverrideJumpAndGravity()
    {
        bool jumpValue = inputManager.GetOverworldJump();
        JumpAndGravity(jumpValue);
    }
    private void OverrideMove()
    {
        Vector2 moveInput = inputManager.GetOverworldMovement();
        if(moveInput != Vector2.zero)
        {
            followPoint.localPosition = new Vector3(-moveInput.x * followPointDistance, 0, -moveInput.y * followPointDistance / 2);
        }
        bool isSprinting = inputManager.GetOverworldSprint();
        Move(moveInput, isSprinting);
    }
}
