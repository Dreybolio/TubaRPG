using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class OverworldHeroFollower : OverworldHero
{
    [Header("Following Data")]
    [SerializeField] private OverworldHeroController heroToFollow;
    [SerializeField] private float distanceUntilFollow;

    private NavMeshPath _currentPath;

    // Vars
    private bool _isFollowing;
    void Start()
    {
        _currentPath = new NavMeshPath();
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
            }
        }
    }
    private void GetPathToHeroController()
    {
        if (NavMesh.CalculatePath(transform.position, heroToFollow.followPoint.position, NavMesh.AllAreas, _currentPath))
        {
            print(_currentPath);
            for (int i = 0; i < _currentPath.corners.Length - 1; i++)
                Debug.DrawLine(_currentPath.corners[i], _currentPath.corners[i + 1], Color.red);
        }
    }
}
