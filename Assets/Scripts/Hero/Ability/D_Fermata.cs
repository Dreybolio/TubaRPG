using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D_Fermata : AbilityBase
{
    public override void DoAbility(GenericEnemy target)
    {
        StartCoroutine(C_DoAbility(target));
    }

    public override IEnumerator C_DoAbility(GenericEnemy target)
    {
        hero.SubtractNP(npCost);
        // Do Pre-Anim
        yield return new WaitForSeconds(0.25f);

        // Do minigame to see if hit
        GameObject mgObject = Instantiate(minigame, hero.minigameParent);
        MinigameBase mgScript = mgObject.GetComponent<MinigameBase>();
        mgScript.StartMinigame();
        yield return new WaitUntil(() => mgScript.isComplete);

        // Minigame is over, do damage
        if (mgScript.successLevel == 1)
        {
            target.Damage(2);
            target.AddStatusEffect(StatusEffect.FERMATA, 2);
        }
        else
        {
            target.Damage(1);
        }

        // Do Post-Anim
        yield return new WaitForSeconds(1.50f);
        mgScript.Destroy();
        hero.ActionFinished();
    }
}
