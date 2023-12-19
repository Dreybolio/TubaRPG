using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D_BeautifulTune : AbilityBase
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
        MinigameBase mgScript = mgObject.GetComponent<MinigameBase>(); // We are expecting attackMinigame to be of this type
        mgScript.StartMinigame();
        yield return new WaitUntil(() => mgScript.isComplete);

        // Minigame is over, do damage
        if (mgScript.successLevel == 1)
        {
            hero.AddStatusEffect(StatusEffect.DEFENCEUP, 2);
            GenericHero otherHero = hero.heroIndex == 0 ? battleManager.GetHero(1) : battleManager.GetHero(0);
            otherHero.AddStatusEffect(StatusEffect.DEFENCEUP, 2);
        }

        // Do Post-Anim
        yield return new WaitForSeconds(0.25f);
        mgScript.Destroy();
        hero.ActionFinished();
    }
}
