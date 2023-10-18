using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericHero : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP;
    public int maxNP;

    [Header("Data")]
    public string attackName;
    public string attackDesc;

    public string abilityOneName;
    public string abilityOneDesc;
    public int abilityOneNPCost;
    public bool abilityOneRequiresHeroSelection;
    public bool abilityOneRequiresEnemySelection;

    public string abilityTwoName;
    public string abilityTwoDesc;
    public int abilityTwoNPCost;
    public bool abilityTwoRequiresHeroSelection;
    public bool abilityTwoRequiresEnemySelection;

    public bool canBeSelected = true;
    public bool canBeTargeted = true;
    public bool isAlive = true;
    protected int heroIndex;
    public int actionsRemaining;
    [NonSerialized] public Dictionary<StatusEffect, int> statusEffects = new();

    [Header("Prefabs")]
    protected Transform minigameParent; // This will get set in BattleManager through SetMinigameParent();
    [SerializeField] protected GameObject attackMinigame;
    [SerializeField] protected GameObject abilityOneMinigame;
    [SerializeField] protected GameObject abilityTwoMinigame;

    // Pointers
    protected BattleManager battleManager;
    protected BattleMenuManager bmManager;
    protected GameData gameData;
    protected Animator animator;
    protected BattleLocationReferencer locationReferencer;

    // Vars
    protected int _hp;
    protected int _np;
    protected bool _walkCoroutineFinished;

    // Anim
    private int _animIdle;
    private int _animDie;
    private int _animGreyOut;

    // Misc
    [SerializeField] private Transform postitionAtFront;

    private void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();
        bmManager = FindObjectOfType<BattleMenuManager>();
        animator = GetComponent<Animator>();
        locationReferencer = FindObjectOfType<BattleLocationReferencer>();
        heroIndex = battleManager.GetHeroIndex(this);
        _hp = maxHP; _np = maxNP;
        AssignAnimationIDs();
    }
    public abstract void DoAttack(GenericEnemy target);
    public abstract void DoAbilityOne(GenericEnemy target);
    public abstract void DoAbilityTwo(GenericEnemy target);
    public abstract void CheckEnemy(GenericEnemy target);
    public void UseItem(int itemIndex, GenericEnemy target)
    {
        ActionFinished();
    }
    public void UseItem(int itemIndex, GenericHero target)
    {
        ActionFinished();
    }
    public void DoNothing()
    {
        ActionFinished();
    }

    public void ActionFinished()
    {
        SetActionsRemaining(actionsRemaining - 1);
        battleManager.TurnProcessed();
    }
    public void WalkToTarget(Transform target, float duration)
    {
        _walkCoroutineFinished = false;
        StartCoroutine(C_WalkToTarget(target, duration));
    }
    private IEnumerator C_WalkToTarget(Transform target, float duration)
    {
        Vector3 startPos = transform.position;
        float timeElapsed = 0;
        while(timeElapsed <= duration)
        {
            transform.position = Vector3.Lerp(startPos, target.position, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = target.position;
        _walkCoroutineFinished = true;
    }
    public void Damage(int damage)
    {
        _hp -= damage;
        print(name + " HP: " + _hp + " (Dealt " + damage + ")");
        if(_hp <= 0)
        {
            battleManager.AddPostTurnEvent(PostTurnEvent.HERO_DIED, this);
        }
        else
        {
            bmManager.SetHeroHP(heroIndex, _hp);
            // Wake up if asleep
            if (statusEffects.ContainsKey(StatusEffect.ASLEEP))
            {
                RemoveStatusEffect(StatusEffect.ASLEEP);
            }
        }
    }
    public void Kill()
    {
        StartCoroutine(C_Kill());
    }
    private IEnumerator C_Kill()
    {
        _hp = 0;
        bmManager.SetHeroHP(heroIndex, 0);
        isAlive = false;
        canBeSelected = false;
        canBeTargeted = false;
        animator.SetTrigger(_animDie);
        yield return new WaitForSeconds(2);
        // After animation is done
    }
    public void SetActionsRemaining(int amount)
    {
        actionsRemaining = amount;
        if (actionsRemaining >= 1)
        {
            canBeSelected = true;
            SetGreyOut(false);
        }
        else
        {
            canBeSelected = false;
            SetGreyOut(true);
            battleManager.AddPostTurnEvent(PostTurnEvent.HERO_OUT_OF_ACTIONS, this);
        }
    }
    public void SubtractNP(int np)
    {
        _np -= np;
        if( _np < 0 )
        {
            Debug.LogError("ERROR: Used more NP than we have. This should never happen.");
        }
        bmManager.SetHeroNP(heroIndex, _np);
    }
    public int GetNP()
    {
        return _np;
    }
    public Transform GetPositionAtFront()
    {
        return postitionAtFront;
    }
    public void AddStatusEffect(StatusEffect statusEffect, int turns)
    {
        statusEffects.Add(statusEffect, turns);
        bmManager.SetHeroStatusEffects(heroIndex, statusEffects); // Update UI

        if(statusEffect == StatusEffect.DECRESCENDO) { canBeTargeted = false; }
    }
    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        statusEffects.Remove(statusEffect);
        bmManager.SetHeroStatusEffects(heroIndex, statusEffects);

        if (statusEffect == StatusEffect.DECRESCENDO) { canBeTargeted = true; }
    }
    public void SetGreyOut(bool b)
    {
        animator.SetBool(_animGreyOut, b);
    }
    public void SetMinigameParent(Transform mgp)
    {
        minigameParent = mgp;
    }
    private void AssignAnimationIDs()
    {
        _animIdle = Animator.StringToHash("Idle");
        _animDie = Animator.StringToHash("Die");
        _animGreyOut = Animator.StringToHash("GreyOut");
    }
}
