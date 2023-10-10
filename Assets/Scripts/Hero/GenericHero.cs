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

    public string specialOneName;
    public string specialOneDesc;
    public int specialOneNPCost;
    public bool specialOneRequiresHeroSelection;
    public bool specialOneRequiresEnemySelection;

    public string specialTwoName;
    public string specialTwoDesc;
    public int specialTwoNPCost;
    public bool specialTwoRequiresHeroSelection;
    public bool specialTwoRequiresEnemySelection;

    public bool canBeSelected = true;
    public bool canBeTargeted = true;
    public bool isAlive = true;
    public int actionsRemaining;
    public int heroIndex;

    [Header("Prefabs")]
    protected Transform minigameParent; // This will get set in BattleManager through SetMinigameParent();
    [SerializeField] protected GameObject attackMinigame;
    [SerializeField] protected GameObject abilityOneMinigame;
    [SerializeField] protected GameObject abilityTwoMinigame;

    // Pointers
    protected BattleManager battleManager;
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

    private void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();
        animator = GetComponent<Animator>();
        locationReferencer = FindObjectOfType<BattleLocationReferencer>();
        _hp = maxHP; _np = maxNP;
        AssignAnimationIDs();
    }
    public void DoAttack(GenericEnemy target)
    {
        ActionFinished();
    }
    public void DoAbilityOne(GenericEnemy target)
    {
        ActionFinished();
    }
    public void DoAbilityTwo(GenericEnemy target)
    {
        ActionFinished();
    }
    public void UseItem(int itemIndex, GenericEnemy target)
    {
        ActionFinished();
    }
    public void UseItem(int itemIndex, GenericHero target)
    {
        ActionFinished();
    }
    public void CheckEnemy(GenericEnemy target)
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
            battleManager.UpdateHeroHealthUI(this, _hp);
        }
    }
    public void Kill()
    {
        StartCoroutine(C_Kill());
    }
    private IEnumerator C_Kill()
    {
        _hp = 0;
        battleManager.UpdateHeroHealthUI(this, 0);
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
        battleManager.UpdateHeroNotePointUI(this, _np);
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
