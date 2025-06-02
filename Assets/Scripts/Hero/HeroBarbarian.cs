using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBarbarian : GenericHero
{
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
}
