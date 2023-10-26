using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroDruid : GenericHero
{
    public override void DoAttack(GenericEnemy target)
    {
        StartCoroutine(C_DoAttack(target));
    }
    private IEnumerator C_DoAttack(GenericEnemy target)
    {
        // Do Pre-Anim
        yield return new WaitForSeconds(0.25f);

        // Do minigame to see if hit
        GameObject mgObject = Instantiate(attackMinigame, minigameParent);
        Minigame_LeftRightAlternate mgScript = mgObject.GetComponent<Minigame_LeftRightAlternate>(); // We are expecting attackMinigame to be of this type
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

        // Do Post-anim
        yield return new WaitForSeconds(0.25f);
        mgScript.Destroy();
        ActionFinished();
    }

    public override void DoAbilityOne(GenericEnemy target)
    {
        StartCoroutine(C_DoAbilityOne(target));
    }
    private IEnumerator C_DoAbilityOne(GenericEnemy target)
    {
        // Do Pre-Anim
        yield return new WaitForSeconds(0.25f);

        // Do minigame to see if hit
        GameObject mgObject = Instantiate(abilityOneMinigame, minigameParent);
        Minigame_SpamA mgScript = mgObject.GetComponent<Minigame_SpamA>(); // We are expecting attackMinigame to be of this type
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
        yield return new WaitForSeconds(0.25f);
        mgScript.Destroy();
        ActionFinished();
    }

    public override void DoAbilityTwo(GenericEnemy target)
    {
        StartCoroutine(C_DoAbilityTwo(target));
    }
    private IEnumerator C_DoAbilityTwo(GenericEnemy target)
    {
        // Do Pre-Anim
        yield return new WaitForSeconds(0.25f);

        // Do minigame to see if hit
        GameObject mgObject = Instantiate(abilityTwoMinigame, minigameParent);
        Minigame_TripleBar mgScript = mgObject.GetComponent<Minigame_TripleBar>(); // We are expecting attackMinigame to be of this type
        mgScript.StartMinigame();
        yield return new WaitUntil(() => mgScript.isComplete);

        // Minigame is over, do damage
        if (mgScript.successLevel == 1)
        {
            AddStatusEffect(StatusEffect.DEFENCEUP, 2);
            GenericHero otherHero = heroIndex == 0 ? battleManager.GetHero(1) : battleManager.GetHero(0);
            otherHero.AddStatusEffect(StatusEffect.DEFENCEUP, 2);
        }

        // Do Post-Anim
        yield return new WaitForSeconds(0.25f);
        mgScript.Destroy();
        ActionFinished();
    }

    public override void CheckEnemy(GenericEnemy target)
    {
        throw new System.NotImplementedException();
    }
}
