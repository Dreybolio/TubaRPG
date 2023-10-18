using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy_TubaKnight : GenericEnemy
{
    
    public new void ProcessTurn()
    {
        base.ProcessTurn();
        target = null;
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
        // Walk to target
        WalkToTarget(target.GetPositionAtFront(), 1f);
        yield return new WaitUntil(() => _walkCoroutineFinished);
        yield return new WaitForSeconds(0.25f);

        // Do animation, allow for blocking
        bool blockSuccessful = false;
        if (!blockSuccessful)
        {
            target.Damage(2);
        }
        else
        {
            target.Damage(1);
        }

        // Walk back to spot
        WalkToTarget(locationReferencer.enemySpawns[enemyIndex], 1f);
        yield return new WaitUntil(() => _walkCoroutineFinished);
        yield return new WaitForSeconds(0.25f);
        FinishTurn();
    }
    private IEnumerator DoCrush()
    {
        // Play animation
        // Allow for block
        yield return new WaitForSeconds(1);
        target.Damage(2);
        yield return new WaitForSeconds(0.5f);
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
