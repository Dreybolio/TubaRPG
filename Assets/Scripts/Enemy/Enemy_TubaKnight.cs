using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_TubaKnight : GenericEnemy
{
    
    public new void ProcessTurn()
    {
        base.ProcessTurn();
        currentTarget = null;
        while (currentTarget == null)
        {
            int r = Random.Range(0, 2);
            if (targets[r].canBeTargeted)
            {
                currentTarget = targets[r];
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
        // Play animation
        // Allow for block
        yield return new WaitForSeconds(1);
        currentTarget.Damage(2);
        yield return new WaitForSeconds(0.5f);
        FinishTurn();
    }
    private IEnumerator DoCrush()
    {
        // Play animation
        // Allow for block
        yield return new WaitForSeconds(1);
        currentTarget.Damage(2);
        yield return new WaitForSeconds(0.5f);
        FinishTurn();
    }
    private IEnumerator DoSummonTrumpet()
    {
        // Check if space is available
        // Summon new guy and tell the BattleManager about it
        yield return new WaitForSeconds(1);
        currentTarget.Damage(2);
        yield return new WaitForSeconds(0.5f);
        FinishTurn();
    }
}
