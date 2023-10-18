using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroRogue : GenericHero
{
    // Note: ALL actions must end with ActionFinished();
    public override void DoAttack(GenericEnemy target)
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
        Minigame_WaitPressA mgScript = mgObject.GetComponent<Minigame_WaitPressA>(); // We are expecting attackMinigame to be of this type
        mgScript.StartMinigame();
        yield return new WaitUntil(() => mgScript.isComplete);

        bool hasDecrescendo = statusEffects.ContainsKey(StatusEffect.DECRESCENDO);
        int decrescedoBonus = hasDecrescendo ? 3 : 1;
        // Minigame is over, do damage
        if (mgScript.successLevel == 1)
        {
            target.Damage(2 * decrescedoBonus);
        }
        else
        {
            target.Damage(1 * decrescedoBonus);
        }
        if (hasDecrescendo) { RemoveStatusEffect(StatusEffect.DECRESCENDO); }

        // Walk back to spot
        WalkToTarget(locationReferencer.heroSpawns[battleManager.GetHeroIndex(this)], 1f);
        yield return new WaitUntil(() => _walkCoroutineFinished);
        yield return new WaitForSeconds(0.25f);
        mgScript.Destroy();
        ActionFinished();
    }
    public override void DoAbilityOne(GenericEnemy target)
    {
        StartCoroutine(C_DoAbilityOne());
    }
    private IEnumerator C_DoAbilityOne()
    {
        SubtractNP(abilityOneNPCost);
        yield return new WaitForSeconds(0.80f);

        GameObject mgObject = Instantiate(abilityOneMinigame, minigameParent);
        Minigame_5Button mgScript = mgObject.GetComponent<Minigame_5Button>(); // We are expecting attackMinigame to be of this type
        mgScript.StartMinigame();
        yield return new WaitUntil(() => mgScript.isComplete);

        if(mgScript.successLevel == 1)
        {
            AddStatusEffect(StatusEffect.DECRESCENDO, -1);
        }

        yield return new WaitForSeconds(1f);
        mgScript.Destroy();
        ActionFinished();
    }
    public override void DoAbilityTwo(GenericEnemy target)
    {
        StartCoroutine(C_DoAbilityTwo(target));
    }
    public IEnumerator C_DoAbilityTwo(GenericEnemy target)
    {
        SubtractNP(abilityTwoNPCost);
        yield return new WaitForSeconds(0.80f);

        GameObject mgObject = Instantiate(abilityTwoMinigame, minigameParent);
        Minigame_5ButtonTimed mgScript = mgObject.GetComponent<Minigame_5ButtonTimed>(); // We are expecting attackMinigame to be of this type
        mgScript.StartMinigame();
        yield return new WaitUntil(() => mgScript.isComplete);

        if (mgScript.successLevel == 1)
        {
            target.AddStatusEffect(StatusEffect.ASLEEP, 1);
        }

        yield return new WaitForSeconds(1f);
        mgScript.Destroy();
        ActionFinished();
    }
    public override void CheckEnemy(GenericEnemy target)
    {
        throw new System.NotImplementedException();
    }
}
