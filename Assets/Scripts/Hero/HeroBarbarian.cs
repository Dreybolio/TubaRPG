using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBarbarian : GenericHero
{
    // Note: ALL actions must end with ActionFinished();
    public new void DoAttack(GenericEnemy target)
    {
        StartCoroutine(C_DoAttack(target));
    }
    private IEnumerator C_DoAttack(GenericEnemy target)
    {
        // Walk to target
        WalkToTarget(target.GetPositionAtFront(), 1f);
        yield return new WaitUntil(() => _walkCoroutineFinished);
        yield return new WaitForSeconds(0.25f);

        // Do minigame to see if hit
        GameObject mgObject = Instantiate(attackMinigame, minigameParent);
        Minigame_HoldLeft mgScript = mgObject.GetComponent<Minigame_HoldLeft>(); // We are expecting attackMinigame to be of this type
        mgScript.StartMinigame();
        yield return new WaitUntil(() => mgScript.isComplete);

        // Minigame is over, do damage
        if (mgScript.successLevel == 1)
        {
            target.Damage(2);
        }
        else
        {
            target.Damage(1);
        }

        // Walk back to spot
        WalkToTarget(locationReferencer.heroSpawns[heroIndex], 1f);
        yield return new WaitUntil(() => _walkCoroutineFinished);
        yield return new WaitForSeconds(0.25f);
        mgScript.Destroy();
        ActionFinished();
    }
    public new void DoAbilityOne(GenericEnemy target)
    {
        StartCoroutine(C_DoAbilityOne(target));
    }
    private IEnumerator C_DoAbilityOne(GenericEnemy target)
    {
        SubtractNP(1);
        // Walk to target
        WalkToTarget(target.GetPositionAtFront(), 1f);
        yield return new WaitUntil(() => _walkCoroutineFinished);
        yield return new WaitForSeconds(0.25f);

        // Do minigame to see if hit
        GameObject mgObject = Instantiate(abilityOneMinigame, minigameParent);
        Minigame_DoubleHoldRelease mgScript = mgObject.GetComponent<Minigame_DoubleHoldRelease>(); // We are expecting attackMinigame to be of this type
        mgScript.StartMinigame();
        yield return new WaitUntil(() => mgScript.isComplete);

        // Minigame is over, do damage
        if (mgScript.successLevel == 2)
        {
            target.Damage(4);
        }
        else if (mgScript.successLevel == 1)
        {
            target.Damage(3);
        }
        else
        {
            target.Damage(2);
        }

        // Walk back to spot
        WalkToTarget(locationReferencer.heroSpawns[heroIndex], 1f);
        yield return new WaitUntil(() => _walkCoroutineFinished);
        yield return new WaitForSeconds(0.25f);
        mgScript.Destroy();
        ActionFinished();
    }
    public new void DoAbilityTwo(GenericEnemy target)
    {
        ActionFinished();
    }
}
