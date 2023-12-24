using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
public enum EnemyOverrideStatus
{
    NO_OVERRIDE,
    ATTACK_OTHER_ENEMY,
}
public abstract class GenericEnemy : MonoBehaviour
{
    // Personal Data
    public new string name;
    [SerializeField] private int maxHP;
    public int defence;

    public int hp;
    [NonSerialized] public Dictionary<StatusEffect, int> statusEffects = new();
    protected int enemyIndex;
    public bool dontDestroyAfterDeath;

    [Header("Attacking Data")]
    public float[] attackOptionChances;
    protected GenericHero[] targets;
    protected GenericHero target;

    [Header("Positional Data")]
    [SerializeField] protected Transform projectileSpawnPos;
    [SerializeField] protected Transform postitionAtFront;
    [SerializeField] protected Transform postitionAtTop;
    [SerializeField] protected Transform positionAtCenter;

    [Header("Sounds")]
    [SerializeField] protected AudioClip sndHurt;

    // Pointers
    protected BattleManager battleManager;
    protected BattleMenuManager bmManager;
    protected Animator animator;
    protected BattleLocationReferencer locationReferencer;
    protected SoundManager soundManager;

    // Vars
    protected bool _walkCoroutineFinished;

    // Animation IDs
    protected int _animIdle_T, _animDie_T, _animWalking_B;
    // Animation Toggled Vars
    protected bool _damageFramePassed;
    protected bool _projectileSpawnFramePassed;
    protected bool _deathAnimFinished;


    protected void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();
        bmManager = FindObjectOfType<BattleMenuManager>();
        animator = GetComponent<Animator>();
        locationReferencer = FindObjectOfType<BattleLocationReferencer>();
        soundManager = SoundManager.Instance;
        enemyIndex = battleManager.GetEnemyIndex(this);
        SetPossibleTargets();
    }
    public EnemyOverrideStatus CheckForOverrides()
    {
        if (statusEffects.ContainsKey(StatusEffect.DIZZY))
        {
            return EnemyOverrideStatus.ATTACK_OTHER_ENEMY;
        }
        return EnemyOverrideStatus.NO_OVERRIDE;
    }
    public abstract void ProcessTurn();
    protected abstract IEnumerator C_AttackOtherEnemy();
    protected void FinishTurn()
    {
        battleManager.TurnProcessed();
    }
    public void Damage(int damage)
    {
        int tempDefence = 0;
        if (statusEffects.ContainsKey(StatusEffect.DEFENCEUP)) { tempDefence += 1; }
        int totalInflicted = Mathf.Clamp(damage - defence - tempDefence, 0, 999);
        hp -= totalInflicted;
        if (hp <= 0)
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
        bmManager.SpawnDamageIndicator(Camera.main.WorldToScreenPoint(postitionAtTop.position), DamageIndicatorType.ENEMY, totalInflicted);
        soundManager.PlaySound(sndHurt, 1, true);
    }
    public void Kill()
    {
        animator.SetTrigger(_animDie_T);
        // This animation is called by the BattleManager to delete (if appropriate)
        // The BattleManager then reads _deathAnimFinished to know to delete this object
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
        for (int i = 0; i < attackOptionChances.Length; i++)
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
    protected List<GenericEnemy> GetEnemiesOtherThanSelf()
    {
        List<GenericEnemy> list = battleManager.GetAllEnemies();
        list.Remove(this);
        return list;
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
        animator.SetBool(_animWalking_B, true);
        while (timeElapsed <= duration)
        {
            transform.position = Vector3.Lerp(startPos, target.position, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = target.position;
        _walkCoroutineFinished = true;
        animator.SetBool(_animWalking_B, false);
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
    public void ProjectileSpawnFramePassed()
    {
        _projectileSpawnFramePassed = true;
    }
    public void DeathAnimationOver()
    {
        _deathAnimFinished = true;
    }
    public bool GetDeathAnimationOver()
    {
        return _deathAnimFinished;
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
    protected void AssignAnimationIDs()
    {
        _animIdle_T = Animator.StringToHash("Idle");
        _animDie_T = Animator.StringToHash("Die");
        _animWalking_B = Animator.StringToHash("Walking");
    }
}
