using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericHero : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP;
    public int maxNP;
    public int defence;

    [Header("Data")]
    public HeroType type;

    public HeroAbility attack;
    protected AbilityBase attackScript;
    public HeroAbility abilityOne;
    protected AbilityBase abilityOneScript;
    public HeroAbility abilityTwo;
    protected AbilityBase abilityTwoScript;

    [SerializeField] protected Material material;

    [NonSerialized] public Dictionary<StatusEffect, int> statusEffects = new();

    [NonSerialized] public int heroIndex;
    [NonSerialized] public Transform minigameParent; // This will get set in BattleManager through SetMinigameParent();

    [Header("Positional Data")]
    [SerializeField] protected Transform projectileSpawnPos;
    [SerializeField] protected Transform postitionAtFront;
    [SerializeField] protected Transform postitionAtTop;
    [SerializeField] protected Transform positionAtCenter;

    [Header("Sounds")]
    [SerializeField] protected AudioClip sndHurt;
    [SerializeField] protected AudioClip sndBlocked;

    // Pointers
    protected BattleManager battleManager;
    protected BattleMenuManager bmManager;
    protected GameData gameData;
    protected Animator animator;
    protected BattleLocationReferencer locationReferencer;
    protected SoundManager soundManager;

    // Vars
    [NonSerialized] public bool canBeSelected = true;
    [NonSerialized] public bool canBeTargeted = true;
    [NonSerialized] public bool isAlive = true;
    [NonSerialized] public bool isBlocking = false;
    [NonSerialized] public int actionsRemaining;
    [NonSerialized] public bool allowBlocking = false;
    protected int _hp;
    protected int _np;
    [NonSerialized] public bool _walkCoroutineFinished;
    private readonly float BLOCK_TIME = 0.40f;

    // Anim
    private int _animIdle_T, _animDie_T, _animBlock_T, _animWalking_B;
    // Animation Toggled Vars
    protected bool _deathAnimFinished;

    private void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();
        bmManager = FindObjectOfType<BattleMenuManager>();
        animator = GetComponent<Animator>();
        locationReferencer = FindObjectOfType<BattleLocationReferencer>();
        soundManager =SoundManager.Instance;
        heroIndex = battleManager.GetHeroIndex(this);
        _hp = maxHP; _np = maxNP;
        CreateAbilityObjects();
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
    public void DoBlock()
    {
        StartCoroutine(C_DoBlock());
    }
    private IEnumerator C_DoBlock()
    {
        if(allowBlocking)
        {
            allowBlocking = false; // Disable functionality after doing this once.
            isBlocking = true;
            animator.SetTrigger(_animBlock_T);
            yield return new WaitForSeconds(BLOCK_TIME);
            isBlocking = false;
        }
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
        animator.SetBool(_animWalking_B, true);
        while(timeElapsed <= duration)
        {
            transform.position = Vector3.Lerp(startPos, target.position, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = target.position;
        animator.SetBool(_animWalking_B, false);
        _walkCoroutineFinished = true;
    }
    public void Damage(int damage)
    {
        int tempDefence = 0;
        if(statusEffects.ContainsKey(StatusEffect.DEFENCEUP)) { tempDefence += 1; }
        int totalInflicted = Mathf.Clamp(damage - defence - tempDefence, 0, 999);
        _hp -= totalInflicted;
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

        // Spawn a damage indicator
        bmManager.SpawnDamageIndicator(Camera.main.WorldToScreenPoint(postitionAtTop.position), DamageIndicatorType.HERO, totalInflicted);
        if (isBlocking || totalInflicted == 0)
        {
            soundManager.PlaySound(sndBlocked, 1, true);
        }
        else
        {
            soundManager.PlaySound(sndHurt, 1, true);
        }
    }
    public void Kill()
    {
        _hp = 0;
        bmManager.SetHeroHP(heroIndex, 0);
        isAlive = false;
        canBeSelected = false;
        canBeTargeted = false;
        animator.SetTrigger(_animDie_T);
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
    public Transform GetPositionAtCenter()
    {
        return positionAtCenter;
    }
    public void DeathAnimationOver()
    {
        _deathAnimFinished = true;
    }
    public bool GetDeathAnimationOver()
    {
        return _deathAnimFinished;
    }
    public void AddStatusEffect(StatusEffect statusEffect, int turns)
    {
        if (!statusEffects.ContainsKey(statusEffect))
        {
            statusEffects.Add(statusEffect, turns);
            bmManager.SetHeroStatusEffects(heroIndex, statusEffects); // Update UI

            if(statusEffect == StatusEffect.DECRESCENDO) { canBeTargeted = false; }
        }
    }
    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        statusEffects.Remove(statusEffect);
        bmManager.SetHeroStatusEffects(heroIndex, statusEffects);

        if (statusEffect == StatusEffect.DECRESCENDO) { canBeTargeted = true; }
    }
    /**
     * Sets whether the Hero is greyed out or not after performing an action
     *  "_Base_Color" is derived from the material property
     */
    public void SetGreyOut(bool b)
    {
        if (b)
        {
            material.SetColor("_Base_Color", Color.grey);
        }
        else
        {
            material.SetColor("_Base_Color", Color.white);
        }
    }
    public void SetMinigameParent(Transform mgp)
    {
        minigameParent = mgp;
    }
    /**
     * Adds components corresponding to each attached HeroAbility.
     * NOTE: The name of each HeroAbility must correspond to an AbilityBase of the same name!
     */
    public void CreateAbilityObjects()
    {
        attackScript = (AbilityBase)gameObject.AddComponent(Type.GetType(attack.className, true));
        attackScript.Initialize(this, attack.minigame, attack.npCost, locationReferencer, battleManager);
        abilityOneScript = (AbilityBase)gameObject.AddComponent(Type.GetType(abilityOne.className, true));
        abilityOneScript.Initialize(this, abilityOne.minigame, abilityOne.npCost, locationReferencer, battleManager);
        abilityTwoScript = (AbilityBase)gameObject.AddComponent(Type.GetType(abilityTwo.className, true));
        abilityTwoScript.Initialize(this, abilityTwo.minigame, abilityTwo.npCost, locationReferencer, battleManager);
    }
    private void AssignAnimationIDs()
    {
        _animIdle_T = Animator.StringToHash("Idle");
        _animDie_T = Animator.StringToHash("Die");
        _animBlock_T = Animator.StringToHash("Block");
        _animWalking_B = Animator.StringToHash("Walking");
    }
}
