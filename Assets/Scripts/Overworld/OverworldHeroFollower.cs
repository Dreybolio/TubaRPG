using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class OverworldHeroFollower : OverworldHero
{
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

    [Header("Following Data")]
    [SerializeField] private OverworldHeroController heroToFollow;
    [SerializeField] private float distanceUntilFollow;
    [SerializeField] private float distanceForRecalculation;

    // Pointers
    private CharacterController controller;
    private Animator animator;

    // Vars
    private NavMeshPath _currentPath;
    private bool _isFollowing;
    private Vector3 _currentFollowTarget;
    private Vector2 _speed;
    private float _verticalVelocity;
    private bool _grounded;

    private readonly float _terminalVelocity = 53.0f;

    // Anim IDs
    private int _animSpeed_F, _animJump_B, _animFreefall_B, _animGrounded_B;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        _currentPath = new();

        AssignAnimationIDs();
    }

    // Update is called once per frame
    void Update()
    {
        if(!_isFollowing)
        {
            if(Vector3.Distance(transform.position, heroToFollow.transform.position) > distanceUntilFollow)
            {
                // Start following the hero
                _isFollowing = true;
                GetPathToHeroController();
            }
        }
        else if(Vector3.Distance(_currentFollowTarget, heroToFollow.followPoint.position) > distanceForRecalculation)
        {
            // Recalculate the position to go to, it's gone too far.
            GetPathToHeroController();
        }
        else if(Vector3.Distance(transform.position, _currentFollowTarget) < 0.25f)
        {
            // We've finished following
            _isFollowing = false;
        }

        print("Is Following: " + _isFollowing + ", Distance to target: " + Vector3.Distance(transform.position, _currentFollowTarget));
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
            // TODO: Implement Jumping if needed
            /*
            if (jumpValue) // If trying to jump + has been long enough since last jump
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravityScale);

                // Animation
                animator.SetBool(_animJump_B, true);
            }
            */
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
        Vector2 moveInput = _isFollowing ? new Vector2(_currentPath.corners[1].x, _currentPath.corners[1].z) - new Vector2(transform.position.x, transform.position.z) :
            Vector2.zero;
        moveInput.Normalize();
        // Set target to 0 if not trying to move.
        float targetSpeed = speed;
        Vector2 targetVector = moveInput * targetSpeed;
        Vector2 currentVector = new(controller.velocity.x, controller.velocity.z);

        // Set goal speed

        if (Vector2.Distance(currentVector, targetVector) < -0.1f || Vector2.Distance(currentVector, targetVector) > 0.1f)
        {
            _speed = Vector2.Lerp(currentVector, targetVector, Time.deltaTime * acceleration);
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
    }

    private void AssignAnimationIDs()
    {
        _animSpeed_F = Animator.StringToHash("Speed");
        _animJump_B = Animator.StringToHash("Jumping");
        _animFreefall_B = Animator.StringToHash("Freefall");
        _animGrounded_B = Animator.StringToHash("Grounded");
    }
    private void GetPathToHeroController()
    {
        if (NavMesh.CalculatePath(transform.position, heroToFollow.followPoint.position, NavMesh.AllAreas, _currentPath))
        {
            for (int i = 0; i < _currentPath.corners.Length - 1; i++)
            {
                Debug.DrawLine(_currentPath.corners[i], _currentPath.corners[i + 1], Color.red);
            }
            _currentFollowTarget = heroToFollow.followPoint.position;
        }
        else
        {
            Debug.Log("Can't find path to hero!");
        }
    }
}
