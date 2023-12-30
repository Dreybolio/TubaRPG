using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.InputSystem;

public class OverworldHeroController : OverworldHero
{
    // Data
    [Header("Movement Data")]
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private float sprintSpeed = 5.0f;
    [SerializeField] private float acceleration = 10.0f;

    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravityScale = -15.0f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private float groundCheckRadius = 0.28f;
    [SerializeField] private LayerMask groundLayers;

    [Header("Positional Data")]
    public Transform followPoint; 
    // Pointers
    private InputManager inputManager;
    private Animator animator;
    private Transform t;
    private CharacterController controller;

    // Vars
    private Vector2 _speed;
    private float _verticalVelocity;
    private bool _grounded;

    private readonly float _terminalVelocity = 53.0f;

    // Anim IDs
    private int _animSpeed_F, _animJump_B, _animFreefall_B, _animGrounded_B;

    private void Start()
    {
        inputManager = InputManager.Instance;
        animator = GetComponentInChildren<Animator>();
        t = transform;
        controller = GetComponent<CharacterController>();

        AssignAnimationIDs();
    }
    private void Update()
    {
        JumpAndGravity();
        GroundedCheck();
        Move();
        ApplyVelocity();
    }
    private void JumpAndGravity()
    {
        
        if (_grounded)
        {
            // Animator: We are not jumping or falling
            animator.SetBool(_animJump_B, false);
            animator.SetBool(_animFreefall_B, false);

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }
            bool jumpValue = inputManager.GetOverworldJump();
            if (jumpValue) // If trying to jump + has been long enough since last jump
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravityScale);

                // Animation
                animator.SetBool(_animJump_B, true);
            }
        }
        else
        {
            animator.SetBool(_animFreefall_B, true);
        }
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += gravityScale * Time.deltaTime;
        }
        
    }
    private void GroundedCheck()
    {
        _grounded = Physics.CheckSphere(groundCheckPos.position, groundCheckRadius, groundLayers, QueryTriggerInteraction.Ignore);
        animator.SetBool(_animGrounded_B, _grounded);
    }
    private void Move()
    {
        Vector2 moveInput = inputManager.GetOverworldMovement();
        // Set target to 0 if not trying to move.
        float targetSpeed = inputManager.GetOverworldSprint() ? sprintSpeed : speed;
        Vector2 targetVector = moveInput.normalized * targetSpeed;
        Vector2 currentVector = new Vector2(controller.velocity.x, controller.velocity.z);


        float inputMagnitude = inputManager.GetControlScheme() != "Keyboard" ?
            moveInput.magnitude : 1f;

        // Set goal speed

        if (Vector2.Distance(currentVector, targetVector) < -0.1f || Vector2.Distance(currentVector, targetVector) > 0.1f)
        {
            _speed = Vector2.Lerp(currentVector, targetVector, Time.deltaTime * acceleration * inputMagnitude);
            _speed = Vector2.ClampMagnitude(_speed, sprintSpeed);
            _speed.Set(Mathf.Round(_speed.x * 1000f) / 1000f, Mathf.Round(_speed.y * 1000f) / 1000f);
        }
        else
        {
            _speed = targetVector;
        }

        //Animation
        // Return 0 - 1 on speed
        animator.SetFloat(_animSpeed_F, _speed.magnitude / speed);
    }
    private void ApplyVelocity()
    {
        controller.Move(new Vector3(_speed.x, _verticalVelocity, _speed.y) * Time.deltaTime);
        //rb.velocity = new Vector3(_speed.x, _verticalVelocity, _speed.y) * Time.deltaTime ;
    }

    private void AssignAnimationIDs()
    {
        _animSpeed_F = Animator.StringToHash("Speed");
        _animJump_B = Animator.StringToHash("Jumping");
        _animFreefall_B = Animator.StringToHash("Freefall");
        _animGrounded_B = Animator.StringToHash("Grounded");
    }
}
