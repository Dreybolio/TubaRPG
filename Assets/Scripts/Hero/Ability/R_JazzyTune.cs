using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_JazzyTune : AbilityBase
{
    public override void DoAbility(GenericEnemy target)
    {
        StartCoroutine(C_DoAbility(target));
    }
    public override IEnumerator C_DoAbility(GenericEnemy target)
    {
        hero.SubtractNP(npCost);
        yield return new WaitForSeconds(0.80f);

        GameObject mgObject = Instantiate(minigame, hero.minigameParent);
        MinigameBase mgScript = mgObject.GetComponent<MinigameBase>(); // We are expecting attackMinigame to be of this type
        mgScript.StartMinigame();
        yield return new WaitUntil(() => mgScript.isComplete);

        if (mgScript.successLevel == 1)
        {
            target.AddStatusEffect(StatusEffect.ASLEEP, 1);
        }

        yield return new WaitForSeconds(1f);
        mgScript.Destroy();
        hero.ActionFinished();
    }

}
