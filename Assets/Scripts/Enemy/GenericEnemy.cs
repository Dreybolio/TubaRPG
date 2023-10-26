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
    protected GenericHero target;


    // Pointers
    protected BattleManager battleManager;
    protected BattleMenuManager bmManager;
    protected Animator animator;
    protected BattleLocationReferencer locationReferencer;

    // Vars
    protected bool _walkCoroutineFinished;
    protected bool _damageFramePassed;

    // Animation IDs
    protected int _animIdle;
    protected int _animDie;

    // Misc
    [SerializeField] private Transform postitionAtFront;
    [SerializeField] private Transform postitionAtTop;

    protected void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();
        bmManager = FindObjectOfType<BattleMenuManager>();
        animator = GetComponent<Animator>();
        locationReferencer = FindObjectOfType<BattleLocationReferencer>();
        enemyIndex = battleManager.GetEnemyIndex(this);
        SetPossibleTargets();
    }
    public void ProcessTurn()
    {

    }
    protected void FinishTurn()
    {
        battleManager.TurnProcessed();
    }
    public void Damage(int dmg)
    {
        hp -= dmg;
        if(hp <= 0)
        {
            bmManager.SetEnemyHPBarValue(enemyIndex, 0);
            battleManager.AddPostTurnEvent(PostTurnEvent.ENEMY_DIED, this);
        }
        else
        {
            bmManager.SetEnemyHPBarValue(enemyIndex, hp);
            // Wake up if asleep
            if (statusEffects.ContainsKey(StatusEffect.ASLEEP))
            {
                RemoveStatusEffect(StatusEffect.ASLEEP);
            }
        }

        // Spawn a damage indicator
        bmManager.SpawnDamageIndicator(Camera.main.WorldToScreenPoint(postitionAtTop.position), DamageIndicatorType.ENEMY, dmg);
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
    public void WalkToTarget(Transform target, float duration)
    {
        _walkCoroutineFinished = false;
        StartCoroutine(C_WalkToTarget(target, duration));
    }
    private IEnumerator C_WalkToTarget(Transform target, float duration)
    {
        Vector3 startPos = transform.position;
        float timeElapsed = 0;
        while (timeElapsed <= duration)
        {
            transform.position = Vector3.Lerp(startPos, target.position, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = target.position;
        _walkCoroutineFinished = true;
    }
    public void AddStatusEffect(StatusEffect statusEffect, int turns)
    {
        if (!statusEffects.ContainsKey(statusEffect))
        {
            statusEffects.Add(statusEffect, turns);
            bmManager.SetEnemyStatusEffects(enemyIndex, statusEffects); // Update UI
        }
    }
    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        statusEffects.Remove(statusEffect);
        bmManager.SetEnemyStatusEffects(enemyIndex, statusEffects);
    }
    public void DamageFramePassed()
    {
        _damageFramePassed = true;
    }
    protected void AssignAnimationIDs()
    {
        _animIdle = Animator.StringToHash("Idle");
        _animDie = Animator.StringToHash("Die");
    }
}
