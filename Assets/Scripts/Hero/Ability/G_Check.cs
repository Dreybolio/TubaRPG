using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Check : AbilityBase
{
    public override void DoAbility(GenericEnemy target)
    {
        StartCoroutine(C_DoAbility(target));
    }

    public override IEnumerator C_DoAbility(GenericEnemy target)
    {
        yield return new WaitForSeconds(0.25f);

        // Do minigame to see if hit
        GameObject mgObject = Instantiate(minigame, hero.minigameParent);
        MinigameBase mgScript = mgObject.GetComponent<MinigameBase>();
        mgScript.StartMinigame();
        yield return new WaitUntil(() => mgScript.isComplete);

        // Minigame is over, reveal stats?
        if (mgScript.successLevel == 1)
        {
            // Reveal Enemy HP
            // Trigger Hero Dialogue
        }
        yield return new WaitForSeconds(0.25f);
        mgScript.Destroy();
        hero.ActionFinished();
    }


}
