using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public enum BattleStage
{
    TOP_OF_ROUND,
    PLAYER_TURN,
    ENEMY_TURN,
    RESTART_ROUND
}
public enum HeroAction
{
    FIGHT,
    ABILITYONE,
    ABILITYTWO,
    ITEM,
    CHECK,
    DONOTHING
}
public enum PostTurnEvent
{
    HERO_DIED,
    HERO_OUT_OF_ACTIONS,
    ENEMY_DIED,
}
public enum StatusEffect
{
    ATTACKUP,
    ATTACKUP_PLUS,
    DEFENCEUP,
    DEFENCEUP_PLUS,
    DECRESCENDO,
    ASLEEP,
    FERMATA,
    DIZZY,
}
public class BattleManager : MonoBehaviour
{
    // Data
    private BattleStage battleStage;
    private readonly GenericEnemy[] enemyList = new GenericEnemy[3];
    private readonly GenericHero[] heroList = new GenericHero[2];
    [SerializeField] private Transform minigameHolder;

    // Pointers
    private BattleController battleController;
    private GameData gameData;
    private BattleMenuManager bmManager;
    private LevelManager levelManager;
    private BattleLocationReferencer locationReferencer;
    private SoundManager soundManager;

    // Vars
    private bool _turnProcessed = false;
    private GenericHero activeHero;
    private readonly List<Tuple<PostTurnEvent, int>> postTurnEvents = new();
    private readonly List<GenericHero> heroesSetForBlocking = new();
    private void Start()
    {
        battleController = FindObjectOfType<BattleController>();
        gameData = GameData.Instance;
        levelManager = LevelManager.Instance;
        soundManager = SoundManager.Instance;
        bmManager = FindObjectOfType<BattleMenuManager>();
        locationReferencer = FindObjectOfType<BattleLocationReferencer>();
    }
    public void SetBattleData(BattleData bd)
    {
        // Place each hero 1 and assign their values
        GameObject hero1 = Instantiate(gameData.heroOne_BattleObject);
        hero1.transform.position = locationReferencer.heroSpawns[0].position;
        heroList[0] = hero1.GetComponent<GenericHero>();

        // Place each hero 2 and assign their values
        GameObject hero2 = Instantiate(gameData.heroTwo_BattleObject);
        hero2.transform.position = locationReferencer.heroSpawns[1].position;
        heroList[1] = hero2.GetComponent<GenericHero>();

        // Set Hero Values and Set UI Accordingly
        for (int i = 0; i < 2; i++)
        {
            heroList[i].SetMinigameParent(minigameHolder);
            bmManager.SetHeroMaxHP(i, heroList[i].maxHP);
            bmManager.SetHeroHP(i, heroList[i].maxHP);
            bmManager.SetHeroMaxNP(i, heroList[i].maxNP);
            bmManager.SetHeroNP(i, heroList[i].maxNP);
        }
        bmManager.SetHeroIcon(0, gameData.heroOne_Icon);
        bmManager.SetHeroIcon(1, gameData.heroTwo_Icon);

        activeHero = heroList[0];
        bmManager.SetMenuData(activeHero);
        bmManager.SnapSelectionMenuToHero(0);

        // Place each enemy and assign their values
        for (int i = 0; i < 3; i++)
        {
            if (bd.enemies[i] != null)
            {
                GameObject enemy = Instantiate(bd.enemies[i]);
                enemy.transform.position = locationReferencer.enemySpawns[i].position;
                enemyList[i] = enemy.GetComponent<GenericEnemy>();
                bmManager.SetEnemyName(i, enemyList[i].name);
                bmManager.SetEnemyHPBarMaxValue(i, enemyList[i].hp);
                bmManager.SetEnemySelectorValidity(i, true);
            }
            else
            {
                // Set Invisible
                bmManager.SetEnemyHPBarValue(i, 0, false);
                bmManager.SetEnemySelectorValidity(i, false);
            }
        }

        // Start the Music
        if(bd.introMusic != null)
        {
            soundManager.PlayMusicWithIntro(bd.introMusic, bd.music, 0.75f);
        }
        else
        {
            soundManager.PlayMusic(bd.music, true, 0.75f);
        }

        StartCoroutine(C_TopOfRound());
    }
    public void NextStage()
    {
        // Based on the current BattleStage, call the coroutine of the next BattleStage.
        switch (battleStage)
        {
            case BattleStage.TOP_OF_ROUND:
                StartCoroutine(C_PlayerTurn());
                break;
            case BattleStage.PLAYER_TURN:
                StartCoroutine(C_EnemyTurn());
                break;
            case BattleStage.ENEMY_TURN:
                StartCoroutine (C_RestartRound());
                break;
            case BattleStage.RESTART_ROUND:
                StartCoroutine(C_TopOfRound());
                break;
        }
    }
    private IEnumerator C_TopOfRound()
    {
        battleStage = BattleStage.TOP_OF_ROUND;
        // Process all status effects for heroes
        for (int i = 0; i < 2; i++)
        {
            foreach (KeyValuePair<StatusEffect, int> effect in heroList[i].statusEffects.ToList())
            {
                if (effect.Key == StatusEffect.FERMATA) { heroList[i].Damage(1); } // Hero should never be able to get this

                if (effect.Value == -1) { continue; } // Effect length is infinite
                if (effect.Value == 1)
                {
                    // Effect has run out.
                    heroList[i].statusEffects.Remove(effect.Key);
                }
                else
                {
                    heroList[i].statusEffects[effect.Key] = effect.Value - 1;
                }
            }
        }
        // Process all status effects for enemies
        for (int i = 0; i < 3; i++)
        {
            if (enemyList[i]  == null) { continue; }
            foreach (KeyValuePair<StatusEffect, int> effect in enemyList[i].statusEffects.ToList())
            {
                if (effect.Key == StatusEffect.FERMATA) { enemyList[i].Damage(1); }
                
                if(effect.Value == -1) { continue; } // Effect length is infinite
                if (effect.Value == 1)
                {
                    // Effect has run out.
                    enemyList[i].statusEffects.Remove(effect.Key);
                }
                else
                {
                    enemyList[i].statusEffects[effect.Key] = effect.Value - 1;
                }
            }
        }
        // Update Status Effect UI
        for (int i = 0; i < 2; i++)
        {
            bmManager.SetHeroStatusEffects(i, heroList[i].statusEffects);
        }
        for (int i = 0; i < 3; i++)
        {
            if (enemyList[i] == null) { continue; }
            bmManager.SetEnemyStatusEffects(i, enemyList[i].statusEffects);
        }

        // Roll for random events (If any)

        // Process Post-Turn events
        yield return C_ProcessPostTurnEvents();
        NextStage();
    }
    private IEnumerator C_PlayerTurn()
    {
        if (heroList[0].isAlive && !heroList[0].statusEffects.ContainsKey(StatusEffect.ASLEEP))
        {
            heroList[0].SetActionsRemaining(1);
            SetActiveHero(0);
        }
        if (heroList[1].isAlive && !heroList[0].statusEffects.ContainsKey(StatusEffect.ASLEEP))
        {
            heroList[1].SetActionsRemaining(1);
            if (!heroList[0].isAlive)
            {
                SetActiveHero(1);
            }
        }
        battleStage = BattleStage.PLAYER_TURN;
        while (heroList[0].actionsRemaining > 0 || heroList[1].actionsRemaining > 0)
        {
            // The menu must be buffered every time
            bmManager.SetMenuData(activeHero);
            battleController.SetControlType(ControlType.Menu);
            yield return new WaitUntil(() => _turnProcessed);
            _turnProcessed = false;

            // Process Post-Turn events (Once per active hero)
            yield return C_ProcessPostTurnEvents();
        }
        battleController.SetControlType(ControlType.None);
        NextStage();
    }
    private IEnumerator C_EnemyTurn()
    {
        // Recolour the heroes back to normal
        heroList[0].SetGreyOut(false);
        heroList[1].SetGreyOut(false);
        print("Begin enemy turn!");
        battleStage = BattleStage.ENEMY_TURN;
        for (int i = 0; i < enemyList.Length; i++)
        {
            if (enemyList[i] == null || enemyList[i].statusEffects.ContainsKey(StatusEffect.ASLEEP)) { continue; }
            enemyList[i].BroadcastMessage("ProcessTurn");
            yield return new WaitUntil(() => _turnProcessed);
            _turnProcessed = false;

            // Process Post-Turn events (Once per active enemy)
            yield return C_ProcessPostTurnEvents();
        }
        print("End enemy turn");
        yield return new WaitForSeconds(1);
        NextStage();
    }
    private IEnumerator C_RestartRound()
    {
        battleStage = BattleStage.RESTART_ROUND;
        yield return new WaitForSeconds(0.1f);
        NextStage();
    }
    public IEnumerator C_ProcessPostTurnEvents()
    {
        // Process everything that's happened so far.
        foreach (Tuple<PostTurnEvent, int> tuplePair in postTurnEvents)
        {
            switch (tuplePair.Item1)
            {
                case PostTurnEvent.HERO_DIED:
                    print("Processing a Hero's Death");
                    heroList[tuplePair.Item2].Kill();
                    yield return new WaitUntil(() => heroList[tuplePair.Item2].GetDeathAnimationOver());
                    if (OnHeroKilled())
                    {
                        // This returning true means the game has ended.
                        StopAllCoroutines();
                        StartCoroutine(EndBattleLose());
                        yield break;
                    }
                    break;
                case PostTurnEvent.HERO_OUT_OF_ACTIONS:
                    print("Processing a Hero's Lack of Actions");
                    SwapHero();
                    break;
                case PostTurnEvent.ENEMY_DIED:
                    print("Processing an Enemy's Death");
                    enemyList[tuplePair.Item2].Kill();
                    // Wait until the death animation has finished until we move on
                    yield return new WaitUntil(() => enemyList[tuplePair.Item2].GetDeathAnimationOver());
                    if (OnEnemyKilled(tuplePair.Item2))
                    {
                        // This returning true means the game has ended.
                        StopAllCoroutines();
                        StartCoroutine(EndBattleWin());
                        yield break;
                    }
                    break;
            }
        }
        // If we make it to this point, it means the game goes on!
        postTurnEvents.Clear();
    }
    public void TurnProcessed()
    {
        _turnProcessed = true;
    }
    public void SwapHero()
    {
        if(activeHero == heroList[0]) 
        {
            if (heroList[1].canBeSelected)
            {
                activeHero = heroList[1];
                bmManager.SnapSelectionMenuToHero(1);
            }
        } 
        else
        {
            if (heroList[0].canBeSelected)
            {
                activeHero = heroList[0];
                bmManager.SnapSelectionMenuToHero(0);
            }
        }
        bmManager.SetMenuData(activeHero);
    }
    public void AllowHeroBlocking(GenericHero hero)
    {
        battleController.SetControlType(ControlType.Blocking);
        if (!heroesSetForBlocking.Contains(hero))
        {
            heroesSetForBlocking.Add(hero);
        }
        hero.allowBlocking = true;
    }
    public void DisallowHeroBlocking()
    {
        battleController.SetControlType(ControlType.None);
        heroesSetForBlocking.Clear();
        heroesSetForBlocking.TrimExcess();
    }
    public void OnHeroBlocked()
    {
        // This will only happen during the enemy's attack phase.
        foreach (GenericHero hero in heroesSetForBlocking)
        {
            hero.DoBlock();
        }
    }
    public void SetActiveHero(int heroIndex)
    {
        activeHero = heroList[heroIndex];
        bmManager.SnapSelectionMenuToHero(heroIndex);
        bmManager.SetMenuData(activeHero);
    }
    public void RelayActionToHero(HeroAction action, int targetIndex = -1, int optItemIndex = 0)
    {
        // A target index of -1 implies that the target is not relevant, and the effect is general.
        battleController.SetControlType(ControlType.None);
        bmManager.SetUIVisibility(false, false, false, false, false, false); // Disable the whole UI
        GenericEnemy target;
        if(targetIndex == -1) { target = null; } 
        else{ target = enemyList[targetIndex]; }

        switch (action)
        {
            case HeroAction.FIGHT:
                activeHero.DoAttack(target);
                break;
            case HeroAction.ABILITYONE:
                activeHero.DoAbilityOne(target);
                break;
            case HeroAction.ABILITYTWO:
                activeHero.DoAbilityTwo(target);
                break;
            case HeroAction.CHECK:
                activeHero.CheckEnemy(target);
                break;
            case HeroAction.ITEM:
                activeHero.UseItem(optItemIndex, target);
                break;
            case HeroAction.DONOTHING:
                activeHero.DoNothing();
                break;
        }

        // Below is legacy code for sending the appropriate action to the hero based on its class.
        #region
        /*
        if(activeHero is HeroBarbarian heroBarbarian)
        {
            switch (action)
            {
                case HeroAction.FIGHT:
                    heroBarbarian.DoAttack(target);
                    break;
                case HeroAction.ABILITYONE:
                    heroBarbarian.DoAbilityOne(target);
                    break;
                case HeroAction.ABILITYTWO:
                    heroBarbarian.DoAbilityTwo(target);
                    break;
                case HeroAction.CHECK:
                    heroBarbarian.CheckEnemy(target);
                    break;
                case HeroAction.ITEM:
                    heroBarbarian.UseItem(optItemIndex, target);
                    break;
            }
            #endregion
        }
        else if (activeHero is HeroRogue heroRogue)
        {
            #region
            switch (action)
            {
                case HeroAction.FIGHT:
                    heroRogue.DoAttack(target);
                    break;
                case HeroAction.ABILITYONE:
                    heroRogue.DoAbilityOne(target);
                    break;
                case HeroAction.ABILITYTWO:
                    heroRogue.DoAbilityTwo(target);
                    break;
                case HeroAction.CHECK:
                    heroRogue.CheckEnemy(target);
                    break;
                case HeroAction.ITEM:
                    heroRogue.UseItem(optItemIndex, target);
                    break;
            }
            #endregion
        }
        else if (activeHero is HeroDruid heroDruid)
        {
            #region
            switch (action)
            {
                case HeroAction.FIGHT:
                    heroDruid.DoAttack(target);
                    break;
                case HeroAction.ABILITYONE:
                    heroDruid.DoAbilityOne(target);
                    break;
                case HeroAction.ABILITYTWO:
                    heroDruid.DoAbilityTwo(target);
                    break;
                case HeroAction.CHECK:
                    heroDruid.CheckEnemy(target);
                    break;
                case HeroAction.ITEM:
                    heroDruid.UseItem(optItemIndex, target);
                    break;
            }
            #endregion
        }
        else if (activeHero is HeroWizard heroWizard)
        {
            #region
            switch (action)
            {
                case HeroAction.FIGHT:
                    heroWizard.DoAttack(target);
                    break;
                case HeroAction.ABILITYONE:
                    heroWizard.DoAbilityOne(target);
                    break;
                case HeroAction.ABILITYTWO:
                    heroWizard.DoAbilityTwo(target);
                    break;
                case HeroAction.CHECK:
                    heroWizard.CheckEnemy(target);
                    break;
                case HeroAction.ITEM:
                    heroWizard.UseItem(optItemIndex, target);
                    break;
            }
        }
        */
        #endregion
    }
    public void AddPostTurnEvent(PostTurnEvent pte, GenericHero hero)
    {
        print("Hero Event! " + hero.name + " has " + pte);
        int index = GetHeroIndex(hero);
        Tuple<PostTurnEvent, int> t = new(pte, index);
        postTurnEvents.Add(t);
    }
    public void AddPostTurnEvent(PostTurnEvent pte, GenericEnemy enemy)
    {
        print("Enemy Event! " + enemy.name + " has " + pte);
        int index = GetEnemyIndex(enemy);
        Tuple<PostTurnEvent, int> t = new(pte, index);
        postTurnEvents.Add(t);
    }
    private bool OnHeroKilled()
    {
        bool battleLost = true;
        foreach (GenericHero h in heroList)
        {
            if(h.isAlive) { battleLost = false; }
        }
        if (battleLost)
        {
            return true;
        }
        return false;
    }
    private bool OnEnemyKilled(int index)
    {
        bmManager.SetEnemyHPBarValue(index, 0, false);
        bmManager.SetEnemySelectorValidity(index, false);
        bmManager.SetEnemyStatusEffects(index, null);
        if (!enemyList[index].dontDestroyAfterDeath)
        {
            enemyList[index].Destroy();
        }
        enemyList[index] = null;
        bool battleWon = true;
        foreach (GenericEnemy e in enemyList)
        {
            if(e != null) { battleWon = false; }
        }
        if (battleWon)
        {
            return true;
        }
        return false;
    }
    public int GetEnemyIndex(GenericEnemy e)
    {
        int index = -1;
        for (int i = 0; i < enemyList.Length; i++)
        {
            if(e == enemyList[i])
            {
                index = i;
            }
        }
        return index;
    }
    public List<GenericEnemy> GetAllEnemies()
    {
        List<GenericEnemy> list = new();
        foreach (GenericEnemy e in enemyList)
        {
            if(e != null)
            {
                list.Add(e);
            }
        }
        return list;
    }
    public int GetHeroIndex(GenericHero h)
    {
        int index = -1;
        for (int i = 0; i < heroList.Length; i++)
        {
            if (h == heroList[i])
            {
                index = i;
            }
        }
        return index;
    }
    public GenericHero GetHero(int index)
    {
        return heroList[index];
    }
    private IEnumerator EndBattleWin()
    {
        foreach (GenericEnemy e in enemyList)
        {
            if(e != null) 
            {
                e.StopAllCoroutines();
            }
        }
        foreach (GenericHero h in heroList)
        {
            if (h != null)
            {
                h.StopAllCoroutines();
            }
        }
        print("You Won The Battle!");
        soundManager.FadeOutMusic(1);
        yield return new WaitForSeconds(2f);
        battleController.SetControlType(ControlType.None);
        levelManager.LoadScene("CharacterSelect");
    }
    private IEnumerator EndBattleLose()
    {
        foreach (GenericEnemy e in enemyList)
        {
            if (e != null)
            {
                e.StopAllCoroutines();
            }
        }
        foreach (GenericHero h in heroList)
        {
            if(h != null)
            {
                h.StopAllCoroutines();
            }
        }
        print("You Lost The Battle!");
        soundManager.FadeOutMusic(1);
        yield return new WaitForSeconds(2f);
        battleController.SetControlType(ControlType.None);
        levelManager.LoadScene("CharacterSelect");
    }
}
