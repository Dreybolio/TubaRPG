using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroRogue : GenericHero
{
    // Note: ALL actions must end with ActionFinished();
    public override void DoAttack(GenericEnemy target)
    {
        attackScript.DoAbility(target);
    }
    public override void DoAbilityOne(GenericEnemy target)
    {
        abilityOneScript.DoAbility(target);
    }
    public override void DoAbilityTwo(GenericEnemy target)
    {
        abilityTwoScript.DoAbility(target);
    }
    public override void CheckEnemy(GenericEnemy target)
    {
        throw new System.NotImplementedException();
    }
}
