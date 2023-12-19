using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy_TubaKnight : GenericEnemy
{
    // Specific Anim IDs
    private int _animCrush;

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
            StartCoroutine(AttackOtherEnemy());
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
                StartCoroutine(DoNoteProjectile());
                break;
            case 1:
                StartCoroutine(DoCrush());
                break;
            case 2:
                StartCoroutine(DoSummonTrumpet());
                break;
        }
    }
    /**
     *  Projectile attack, X damage on hit.
     */
    private IEnumerator DoNoteProjectile()
    {
        StartCoroutine(DoCrush());
        yield return null;
    }
    /**
     *  Melee attack; 2 damage on hit.
     */
    private IEnumerator DoCrush()
    {
        // Walk to target
        WalkToTarget(target.GetPositionAtFront(), 1f);
        yield return new WaitUntil(() => _walkCoroutineFinished);
        yield return new WaitForSeconds(0.25f);

        battleManager.AllowHeroBlocking(target);
        animator.SetTrigger(_animCrush);
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
     *  Creates a little minion and adds it to the fight
     */
    private IEnumerator DoSummonTrumpet()
    {
        StartCoroutine(DoCrush());
        yield return null;
    }

    /**
     *  Walk to enemy, and guarantee do 2 damage
     */
    protected override IEnumerator AttackOtherEnemy()
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

        animator.SetTrigger(_animCrush);
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
        _animCrush = Animator.StringToHash("Crush");
    }
}
