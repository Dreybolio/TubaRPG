using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityBase : MonoBehaviour
{
    protected GenericHero hero;
    protected GameObject minigame;
    protected int npCost;

    protected BattleLocationReferencer locationReferencer;
    protected BattleManager battleManager;
    public abstract void DoAbility(GenericEnemy target);
    public abstract IEnumerator C_DoAbility(GenericEnemy target);
    public void Initialize(GenericHero gh, GameObject mg, int cost, BattleLocationReferencer blr, BattleManager bm)
    {
        hero = gh;
        minigame = mg;
        npCost = cost;
        locationReferencer = blr;
        battleManager = bm;
    }
}
