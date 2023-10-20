using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy_TubaKnight : GenericEnemy
{
    // Specific Anim IDs
    private int _animCrush;
    public new void ProcessTurn()
    {
        base.ProcessTurn();
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
    private IEnumerator DoNoteProjectile()
    {
        StartCoroutine(DoCrush());
        yield return null;
    }
    private IEnumerator DoCrush()
    {
        // Walk to target
        WalkToTarget(target.GetPositionAtFront(), 1f);
        yield return new WaitUntil(() => _walkCoroutineFinished);
        yield return new WaitForSeconds(0.25f);

        battleManager.AllowHeroBlocking();
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
    private IEnumerator DoSummonTrumpet()
    {
        // Check if space is available
        // Summon new guy and tell the BattleManager about it
        yield return new WaitForSeconds(1);
        target.Damage(2);
        yield return new WaitForSeconds(0.5f);
        FinishTurn();
    }
}
