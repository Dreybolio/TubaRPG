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
        if (mgScript.success)
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
        SubtractNP(1);
        // Do minigame to see if hit
        target.Damage(4);
        ActionFinished();
    }
    public new void DoAbilityTwo(GenericEnemy target)
    {
        ActionFinished();
    }
}
