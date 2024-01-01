using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OverworldEnemyBase : MonoBehaviour
{
    [Header("Movement Data")]
    [SerializeField] protected float speed = 2.5f;
    [SerializeField] protected float acceleration = 10.0f;
    [SerializeField] protected float gravityScale = -15.0f;

    [Header("Player Detection")]
    [SerializeField] protected float visiblityRange = 5.0f;
    [SerializeField] protected float maxWalkingRange = 8.0f;

    [Header("Ground Detection")]
    [SerializeField] protected Transform groundCheckPos;
    [SerializeField] protected float groundCheckRadius = 0.28f;
    [SerializeField] protected LayerMask groundLayers;

    [Header("Battle Data")]
    [SerializeField] protected BattleData battleOnTouch;

    [Header("Pointers")]
    [SerializeField] protected CharacterModel model;
    [SerializeField] protected Animator animator;
    protected CharacterController controller;
    protected LevelManager levelManager;
    protected Hitbox hitbox;
    protected Transform hero;

    // Vars
    protected Vector2 _speed;
    protected float _verticalVelocity;
    protected bool _grounded;
    protected bool _facingRight = true;
    protected bool _facingForward = true;
    protected readonly float _terminalVelocity = 53.0f;
    protected bool _chasingPlayer = false;
    protected Vector3 startPos;
    protected float timer = 0f;

    // Anim IDs
    protected int _animSpeed_F;

    protected void Start()
    {
        controller = GetComponent<CharacterController>();
        startPos = transform.position;
        levelManager = LevelManager.Instance;
        hitbox = GetComponentInChildren<Hitbox>();
        hero = FindObjectOfType<OverworldHeroController>().transform;
        model.SetAsOverworldModel();

        AssignAnimationIDs();
        hitbox.OnHitboxEnter += OnTouchPlayer;
    }

    // Update is called once per frame
    protected void Update()
    {
        if(!_chasingPlayer)
        {
            if(timer >= 0.25f)
            {
                timer = 0f;
                if(CheckForPlayer()) { _chasingPlayer = true; }
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        else
        {
            if (timer >= 0.25f)
            {
                timer = 0f;
                if (CheckOutOfRange()) { _chasingPlayer = false; }
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        DoMovement();
    }

    protected bool CheckForPlayer()
    {
        return Vector3.Distance(startPos, hero.position) <= visiblityRange;
    }
    protected bool CheckOutOfRange()
    {
        return Vector3.Distance(transform.position, startPos) >= maxWalkingRange;
    }
    protected void OnTouchPlayer()
    {
        levelManager.LoadBattle(battleOnTouch);
    }
    protected void AssignAnimationIDs()
    {
        _animSpeed_F = Animator.StringToHash("Speed");
    }
    protected abstract void DoMovement();

}
