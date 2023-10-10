using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroRogue : GenericHero
{
    private bool hasDecrescendoBonus = false;
    // Note: ALL actions must end with ActionFinished();
    public new void DoAttack(GenericEnemy target)
    {
        // Do minigame to see if hit
        // If hit
        int attackMod = hasDecrescendoBonus ? 3 : 1; // If has decrescedo active, deal x3 damage
        target.Damage(2 * attackMod);
        hasDecrescendoBonus = false;
        ActionFinished();
    }
    public new void DoAbilityOne(GenericEnemy target)
    {
        SubtractNP(2);
        // Do minigame
        hasDecrescendoBonus = true;
        ActionFinished();
    }
    public new void DoAbilityTwo(GenericEnemy target)
    {
        ActionFinished();
    }
}
