using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy_TubaKnight : GenericEnemy
{
    // Specific Anim IDs
    private int _animCrush_T, _animNoteProjectile_T, _animNoteBarrage_T;

    [Header("Projectiles")]
    [SerializeField] private GameObject EighthNoteProjectile;
    [SerializeField] private GameObject HalfNoteProjectile;

    protected new void Start()
    {
        base.Start();
        AssignAnimationIDs();
    }
    public override void ProcessTurn()
    {
        EnemyOverrideStatus ovStatus = CheckForOverrides();
        if (ovStatus == EnemyOverrideStatus.ATTACK_OTHER_ENEMY)
        {
            StartCoroutine(C_AttackOtherEnemy());
            return;
        }

        target = null;
        if (!targets[0].canBeTargeted && !targets[1].canBeTargeted)
        {
            // Nobody can be targeted, probably due to Decrescendo if at this point. Just skip your turn.
            FinishTurn();
            return;
        }
        while (target == null)
        {
            int r = Random.Range(0, 2);
            if (targets[r].canBeTargeted)
            {
                target = targets[r];
            }
        }
        int attack = ChooseAttack();
        switch (attack)
        {
            case 0:
                StartCoroutine(C_DoCrush());
                break;
            case 1:
                StartCoroutine(C_DoNoteProjectile());
                break;
            case 2:
                StartCoroutine(C_DoNoteBarrage());
                break;
            case 3:
                StartCoroutine(C_DoSummonTrumpet());
                break;
        }
    }
    /**
     *  Melee attack; 2 damage on hit.
     */
    private IEnumerator C_DoCrush()
    {
        // Walk to target
        WalkToTarget(target.GetPositionAtFront(), 1f);
        yield return new WaitUntil(() => _walkCoroutineFinished);
        yield return new WaitForSeconds(0.25f);

        battleManager.AllowHeroBlocking(target);
        animator.SetTrigger(_animCrush_T);
        _damageFramePassed = false;
        yield return new WaitUntil(() => _damageFramePassed);
        bool blockSuccessful = target.isBlocking;
        battleManager.DisallowHeroBlocking();
        if (!blockSuccessful)
        {
            target.Damage(2);
        }
        else
        {
            target.Damage(1);
        }
        yield return new WaitForSeconds(0.50f);

        // Walk back to spot
        WalkToTarget(locationReferencer.enemySpawns[enemyIndex], 1f);
        yield return new WaitUntil(() => _walkCoroutineFinished);
        yield return new WaitForSeconds(0.25f);
        FinishTurn();
    }
    /**
     *  Projectile attack, 2 damage on hit.
     */
    private IEnumerator C_DoNoteProjectile()
    {
        animator.SetTrigger(_animNoteProjectile_T);
        _projectileSpawnFramePassed = false;
        yield return new WaitUntil(() => _projectileSpawnFramePassed);
        // Spawn projectile and let it go
        GameObject projObj = Instantiate(HalfNoteProjectile);
        projObj.transform.position = projectileSpawnPos.position;
        ProjectileBase projScript = projObj.GetComponent<ProjectileBase>();
        battleManager.AllowHeroBlocking(target);
        projScript.MoveToTarget(target.GetPositionAtCenter().position, 1.5f);
        yield return new WaitUntil(() => projScript.finishedMovement);
        bool blockSuccessful = target.isBlocking;
        battleManager.DisallowHeroBlocking();
        if (!blockSuccessful)
        {
            target.Damage(2);
        }
        else
        {
            target.Damage(1);
        }
        projScript.Destroy();
        yield return new WaitForSeconds(0.50f);
        FinishTurn();
    }
    /**
     *  3 Projectile attacks, 1 damage on hit.
     */
    private bool[] _noteBarrageSubroutineStatus = new bool[3];
    private IEnumerator C_DoNoteBarrage()
    {
        animator.SetTrigger(_animNoteBarrage_T);
        _projectileSpawnFramePassed = false;
        for (int i = 0; i < 3; i++)
        {
            _noteBarrageSubroutineStatus[i] = false;
            _projectileSpawnFramePassed = false;
            yield return new WaitUntil(() => _projectileSpawnFramePassed);
            if(i == 0)  // Enable blocking on first go, otherwise we will wait until the respective note is gone before enabling again.
            {
                battleManager.AllowHeroBlocking(target);
            }
            // Spawn projectile and let it go
            GameObject projObj = Instantiate(EighthNoteProjectile);
            projObj.transform.position = projectileSpawnPos.position;
            ProjectileBase projScript = projObj.GetComponent<ProjectileBase>();
            StartCoroutine(C_DoNoteBarrage_Subroutine(projScript, i));
        }

        for (int i = 0; i < 3; i++)
        {
            yield return new WaitUntil(() => _noteBarrageSubroutineStatus[i] == true);
        }
        battleManager.DisallowHeroBlocking();
        yield return new WaitForSeconds(0.50f);
        FinishTurn();
    }
    private IEnumerator C_DoNoteBarrage_Subroutine(ProjectileBase projScript, int id)
    {
        projScript.MoveToTarget(target.GetPositionAtCenter().position, 1.5f);
        yield return new WaitUntil(() => projScript.finishedMovement);
        bool blockSuccessful = target.isBlocking;
        battleManager.DisallowHeroBlocking();
        if (!blockSuccessful)
        {
            target.Damage(1);
        }
        else
        {
            target.Damage(0);
        }

        if (id != 2) // If this is not the last one, enable heroblocking for the next one
        {
            battleManager.AllowHeroBlocking(target);
        }
        _noteBarrageSubroutineStatus[id] = true;
        projScript.Destroy();
    }
    /**
     *  Creates a little minion and adds it to the fight
     */
    private IEnumerator C_DoSummonTrumpet()
    {
        StartCoroutine(C_DoCrush());
        yield return null;
    }

    /**
     *  Walk to enemy, and guarantee do 2 damage
     */
    protected override IEnumerator C_AttackOtherEnemy()
    {
        // Choose a target

        List<GenericEnemy> list = GetEnemiesOtherThanSelf();
        if(list.Count <= 0) 
        {
            FinishTurn(); // No other enemies, skip turn instead
            yield break;
        }
        int r = Random.Range(0, list.Count);
        GenericEnemy target = list[r];

        // Process attack vs. target

        WalkToTarget(target.GetPositionAtFront(), 1f);
        yield return new WaitUntil(() => _walkCoroutineFinished);
        yield return new WaitForSeconds(0.25f);

        animator.SetTrigger(_animCrush_T);
        _damageFramePassed = false;
        yield return new WaitUntil(() => _damageFramePassed);
        target.Damage(2);
        yield return new WaitForSeconds(0.50f);

        // Walk back to spot
        WalkToTarget(locationReferencer.enemySpawns[enemyIndex], 1f);
        yield return new WaitUntil(() => _walkCoroutineFinished);
        yield return new WaitForSeconds(0.25f);
        FinishTurn();
    }

    private new void AssignAnimationIDs()
    {
        base.AssignAnimationIDs();
        _animCrush_T = Animator.StringToHash("Crush");
        _animNoteProjectile_T = Animator.StringToHash("NoteProjectile");
        _animNoteBarrage_T = Animator.StringToHash("NoteBarrage");
    }
}
