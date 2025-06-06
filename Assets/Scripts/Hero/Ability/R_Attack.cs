using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_Attack : AbilityBase
{
    public override void DoAbility(GenericEnemy target)
    {
        StartCoroutine(C_DoAbility(target));
    }
    public override IEnumerator C_DoAbility(GenericEnemy target)
    {
        // Walk to target
        hero.WalkToTarget(target.GetPositionAtFront(), 1f);
        yield return new WaitUntil(() => hero._walkCoroutineFinished);
        yield return new WaitForSeconds(0.25f);

        // Do minigame to see if hit
        GameObject mgObject = Instantiate(minigame, hero.minigameParent);
        MinigameBase mgScript = mgObject.GetComponent<MinigameBase>();
        mgScript.StartMinigame();
        yield return new WaitUntil(() => mgScript.isComplete);

        bool hasDecrescendo = hero.statusEffects.ContainsKey(StatusEffect.DECRESCENDO);
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
        if (hasDecrescendo) { hero.RemoveStatusEffect(StatusEffect.DECRESCENDO); }

        // Walk back to spot
        hero.WalkToTarget(locationReferencer.heroSpawns[battleManager.GetHeroIndex(hero)], 1f);
        yield return new WaitUntil(() => hero._walkCoroutineFinished);
        yield return new WaitForSeconds(0.25f);
        mgScript.Destroy();
        hero.ActionFinished();
    }

}
