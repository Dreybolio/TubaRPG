using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericEnemy : MonoBehaviour
{
    // Personal Data
    public new string name;
    [SerializeField] private int maxHP;
    public int hp;
    [NonSerialized] public Dictionary<StatusEffect, int> statusEffects = new();
    protected int enemyIndex;

    // Attacking Data
    public int numAttackOptions;
    public float[] attackOptionChances;
    protected GenericHero[] targets;
    protected GenericHero currentTarget;


    // Pointers
    protected BattleManager battleManager;
    protected BattleMenuManager bmManager;
    protected Animator animator;

    // Animation IDs
    protected int _animIdle;
    protected int _animDie;

    // Misc
    [SerializeField] private Transform postitionAtFront;

    private void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();
        bmManager = FindObjectOfType<BattleMenuManager>();
        animator = GetComponent<Animator>();
        enemyIndex = battleManager.GetEnemyIndex(this);
        SetPossibleTargets();
        AssignAnimationIDs();
    }
    public void ProcessTurn()
    {

    }
    protected void FinishTurn()
    {
        battleManager.TurnProcessed();
    }
    public void Damage(int hp)
    {
        this.hp -= hp;
        if(this.hp <= 0)
        {
            bmManager.SetEnemyHPBarValue(enemyIndex, 0);
            battleManager.AddPostTurnEvent(PostTurnEvent.ENEMY_DIED, this);
        }
        else
        {
            bmManager.SetEnemyHPBarValue(enemyIndex, hp);
        }
    }
    public void Kill()
    {
        StartCoroutine(C_Kill());
    }
    private IEnumerator C_Kill()
    {
        animator.SetTrigger(_animDie);
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
    private void SetPossibleTargets()
    {
        targets = new GenericHero[] 
        { battleManager.GetHero(0), battleManager.GetHero(1) };
    }
    public Transform GetPositionAtFront()
    {
        return postitionAtFront;
    }
    protected int ChooseAttack()
    {
        float f = UnityEngine.Random.Range(0f, 100f);
        float chanceCounter = 0;
        for (int i = 0; i < numAttackOptions; i++)
        {
            chanceCounter += attackOptionChances[i];
            if (f <= chanceCounter)
            {
                // This is our attack
                return i;
            }
        }
        Debug.LogError("Could not choose attack. Did you input attack chances correctly?");
        return -1;
    }
    public void AddStatusEffect(StatusEffect statusEffect, int turns)
    {
        statusEffects.Add(statusEffect, turns);
        bmManager.SetHeroStatusEffects(enemyIndex, statusEffects); // Update UI
    }
    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        statusEffects.Remove(statusEffect);
        bmManager.SetHeroStatusEffects(enemyIndex, statusEffects);
    }
    private void AssignAnimationIDs()
    {
        _animIdle = Animator.StringToHash("Idle");
        _animDie = Animator.StringToHash("Die");
    }
}
