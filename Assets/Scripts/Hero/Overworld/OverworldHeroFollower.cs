using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class OverworldHeroFollower : OverworldHero
{
    [Header("Following Data")]
    [SerializeField] private float rushSpeed = 8.0f;
    [SerializeField] private OverworldHeroController heroToFollow;
    [SerializeField] private float distanceUntilFollow;
    [SerializeField] private float distanceUntilSprint;
    [SerializeField] private float distanceUntilRush;
    [SerializeField] private float distanceUntilTeleport;
    [SerializeField] private float distanceForRecalculation;


    // Vars
    private NavMeshPath _currentPath;
    private bool _isFollowing;
    private Vector3 _currentFollowTarget;
    private new void Start()
    {
        base.Start();
        _currentPath = new();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_modelIsValid)
        {
            Debug.LogError("Warning: HeroFollower has no model or animator!");
            return;
        }
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

        OverrideJumpAndGravity();
        GroundedCheck();
        OverrideMove();
        CheckForTurnAround();
        ApplyVelocity();
    }
    private void OverrideJumpAndGravity()
    {
        JumpAndGravity(false);
    }

    private void OverrideMove()
    {
        Vector2 moveInput = _isFollowing ? new Vector2(_currentPath.corners[1].x, _currentPath.corners[1].z) - new Vector2(transform.position.x, transform.position.z) : Vector2.zero;
        moveInput.Normalize();
        float distanceFromTarget = Vector3.Distance(transform.position, _currentFollowTarget);
        float targetSpeed;
        if(distanceFromTarget > distanceUntilTeleport)
        {
            Debug.Log("Hero got too far away, teleporting...");
            transform.position = heroToFollow.followPoint.position;
            targetSpeed = rushSpeed;
        }
        else if(distanceFromTarget > distanceUntilRush)
        {
            targetSpeed = rushSpeed;
        }
        else if(distanceFromTarget > distanceUntilSprint)
        {
            targetSpeed = sprintSpeed;
        }
        else
        {
            targetSpeed = speed;
        }

        Move(moveInput, targetSpeed);
    }

    private void GetPathToHeroController()
    {
        if (NavMesh.CalculatePath(transform.position, heroToFollow.followPoint.position, NavMesh.AllAreas, _currentPath))
        {
            for (int i = 0; i < _currentPath.corners.Length - 1; i++)
            {
                Debug.DrawLine(_currentPath.corners[i], _currentPath.corners[i + 1], Color.red);
            }
        }
        else
        {
            Debug.LogWarning("Can't find path to hero!");
            // We'll default by drawing the navmesh path ourselves, as a straight line. Who knows if it'll work?
            // Better than shitting yourself panicked and walking off an edge lol.
            _currentPath.ClearCorners();
            _currentPath.corners[1] = heroToFollow.followPoint.position;
        }
        _currentFollowTarget = heroToFollow.followPoint.position;
    }
}
