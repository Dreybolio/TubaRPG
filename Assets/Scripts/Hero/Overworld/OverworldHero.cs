using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class OverworldHero : MonoBehaviour
{
    [Header("Movement Data")]
    [SerializeField] protected float speed = 3.0f;
    [SerializeField] protected float sprintSpeed = 5.0f;
    [SerializeField] protected float acceleration = 10.0f;

    [SerializeField] protected float jumpHeight = 1.2f;
    [SerializeField] protected float gravityScale = -15.0f;

    [Header("Ground Detection")]
    [SerializeField] protected Transform groundCheckPos;
    [SerializeField] protected float groundCheckRadius = 0.28f;
    [SerializeField] protected LayerMask groundLayers;

    private HeroType _type;
    private GameObject _model;

    // Pointers
    protected CharacterController controller;
    protected CharacterModel model;
    protected Animator animator;

    // Vars
    protected Vector2 _speed;
    protected float _verticalVelocity;
    protected bool _grounded;
    protected bool _facingRight = true;
    protected bool _facingForward = true;
    protected readonly float _terminalVelocity = 53.0f;
    protected bool _modelIsValid = false;

    // Anim IDs
    protected int _animSpeed_F, _animJump_B, _animFreefall_B, _animGrounded_B;

    protected void Start()
    {
        controller = GetComponent<CharacterController>();
        AssignAnimationIDs();
    }

    /**
     *  Sets the model. Replaces the old model if it's of a new HeroType
     */
    public void SetCharacterModel(GameObject mdl, HeroType type)
    {
        if(_model != null)
        {
            if(type == _type) { return; }
            Destroy(_model);
        }
        _model = Instantiate(mdl);
        _model.transform.SetParent(transform, false);
        _model.transform.localPosition = Vector3.zero;

        model = _model.GetComponent<CharacterModel>();
        animator = _model.GetComponent<Animator>();
        
        model.SetAsOverworldModel();
        _type = type;
        _modelIsValid = true;
    }
    protected void JumpAndGravity(bool didJump)
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
            if (didJump) // If trying to jump
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
    protected void GroundedCheck()
    {
        _grounded = Physics.CheckSphere(groundCheckPos.position, groundCheckRadius, groundLayers, QueryTriggerInteraction.Ignore);
        animator.SetBool(_animGrounded_B, _grounded);
    }
    protected void Move(Vector2 moveInput, float targetSpeed)
    {
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
    protected void CheckForTurnAround()
    {
        if (_facingRight && _speed.x < 0f)
        {
            model.SetDirection(ModelDirection.LEFT);
            _facingRight = false;
        }
        else if (!_facingRight && _speed.x > 0f)
        {
            model.SetDirection(ModelDirection.RIGHT);
            _facingRight = true;
        }

        if (_facingForward && _speed.y > 0.5f)
        {
            model.SetDirection(ModelDirection.BACKWARD);
            _facingForward = false;
        }
        else if ((!_facingForward && _speed.y < 0f) || (!_facingForward && _speed.y <= 0.5f && _speed.x != 0f))
        {
            model.SetDirection(ModelDirection.FORWARD);
            _facingForward = true;
        }
    }
    protected void ApplyVelocity()
    {
        controller.Move(new Vector3(_speed.x, _verticalVelocity, _speed.y) * Time.deltaTime);
    }

    protected void AssignAnimationIDs()
    {
        _animSpeed_F = Animator.StringToHash("Speed");
        _animJump_B = Animator.StringToHash("Jumping");
        _animFreefall_B = Animator.StringToHash("Freefall");
        _animGrounded_B = Animator.StringToHash("Grounded");
    }
}
