using System;
using System.Collections;
using System.Collections.Generic;
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
    CHECK
}
public enum PostTurnEvent
{
    HERO_DIED,
    HERO_OUT_OF_ACTIONS,
    ENEMY_DIED,
}
public class BattleManager : MonoBehaviour
{
    // Data
    private BattleStage battleStage;
    private BattleData bd;
    private GenericEnemy[] enemyList;
    private GenericHero[] heroList;
    [SerializeField] private Transform minigameHolder;

    // Pointers
    private PlayerControllerBattle playerController;
    private GameData gameData;
    private BattleMenuManager bmManager;
    private LevelManager levelManager;
    private BattleLocationReferencer locationReferencer;

    // Vars
    private bool _turnProcessed = false;
    private GenericHero activeHero;
    private List<Tuple<PostTurnEvent, int>> postTurnEvents;
    private void Start()
    {
        playerController = FindObjectOfType<PlayerControllerBattle>();
        gameData = GameData.Instance;
        levelManager = LevelManager.Instance;
        bmManager = FindObjectOfType<BattleMenuManager>();
        locationReferencer = FindObjectOfType<BattleLocationReferencer>();
        enemyList = new GenericEnemy[3];
        heroList = new GenericHero[2];
        postTurnEvents = new List<Tuple<PostTurnEvent, int>>();
    }
    public void SetBattleData(BattleData bd)
    {
        this.bd = bd;
        // Place each hero 1 and assign their values
        GameObject hero1 = Instantiate(gameData.heroOne_Object);
        hero1.transform.position = locationReferencer.heroSpawns[0].position;
        heroList[0] = hero1.GetComponent<GenericHero>();
        heroList[0].SetMinigameParent(minigameHolder);
        bmManager.SetHeroOneIcon(gameData.heroOne_Icon);
        bmManager.SetHeroOneMaxHP(heroList[0].maxHP);
        bmManager.SetHeroOneHP(heroList[0].maxHP);
        bmManager.SetHeroOneMaxNP(heroList[0].maxNP);
        bmManager.SetHeroOneNP(heroList[0].maxNP);

        // Place each hero 2 and assign their values
        GameObject hero2 = Instantiate(gameData.heroTwo_Object);
        hero2.transform.position = locationReferencer.heroSpawns[1].position;
        heroList[1] = hero2.GetComponent<GenericHero>();
        heroList[1].SetMinigameParent(minigameHolder);
        bmManager.SetHeroTwoIcon(gameData.heroTwo_Icon);
        bmManager.SetHeroTwoMaxHP(heroList[1].maxHP);
        bmManager.SetHeroTwoHP(heroList[1].maxHP);
        bmManager.SetHeroTwoMaxNP(heroList[1].maxNP);
        bmManager.SetHeroTwoNP(heroList[1].maxNP);

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
                enemyList[i].SetPossibleTargets(heroList[0], heroList[1]);
                bmManager.SetEnemyHPBarValue(i, enemyList[i].hp);
                bmManager.SetEnemySelectorValidity(i, enemyList[i] != null);
            }
            else
            {
                // Set Invisible
                bmManager.SetEnemyHPBarValue(i, 0, false);
            }
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

        // Process all status effects, roll for random events (If any)
        yield return new WaitForSeconds(0.1f);
        NextStage();

    }
    private IEnumerator C_PlayerTurn()
    {
        if (heroList[0].isAlive)
        {
            heroList[0].SetActionsRemaining(1);
            SetActiveHero(0);
        }
        if (heroList[1].isAlive)
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
            playerController.SetControlType(ControlType.Menu);
            yield return new WaitUntil(() => _turnProcessed);
            _turnProcessed = false;
        }
        playerController.SetControlType(ControlType.None);
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
            if (enemyList[i] == null) { continue; }
            enemyList[i].BroadcastMessage("ProcessTurn");
            yield return new WaitUntil(() => _turnProcessed);
            _turnProcessed = false;
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

    public void TurnProcessed()
    {
        // Process everything that's happened so far.
        foreach(Tuple<PostTurnEvent, int> tuplePair in postTurnEvents)
        {
            switch (tuplePair.Item1)
            {
                case PostTurnEvent.HERO_DIED:
                    print("Processing a Hero's Death");
                    heroList[tuplePair.Item2].Kill();
                    if (OnHeroKilled(tuplePair.Item2))
                    {
                        // This returning true means the game has ended.
                        EndBattleLose();
                        return;
                    }
                    break;
                case PostTurnEvent.HERO_OUT_OF_ACTIONS:
                    print("Processing a Hero's Lack of Actions");
                    SwapHero();
                    break;
                case PostTurnEvent.ENEMY_DIED:
                    print("Processing an Enemy's Death");
                    enemyList[tuplePair.Item2].Kill();
                    if (OnEnemyKilled(tuplePair.Item2))
                    {
                        // This returning true means the game has ended.
                        EndBattleWin();
                        return;
                    }
                    break;
            }
        }
        // If we make it to this point, it means the game goes on!
        postTurnEvents.Clear();
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
    public void SetActiveHero(int heroIndex)
    {
        activeHero = heroList[heroIndex];
        bmManager.SnapSelectionMenuToHero(heroIndex);
        bmManager.SetMenuData(activeHero);
    }
    public void RelayActionToHero(HeroAction action, int targetIndex = -1, int optItemIndex = 0)
    {
        // THIS IS TEMPORARY
        // Later on, it is at this stage where minigames will be called before we finalize the action.


        // A target index of -1 implies that the target is not relevant, and the effect is general.
        playerController.SetControlType(ControlType.None);
        bmManager.SetUIVisibility(false, false, false, false); // Disable the whole UI
        GenericEnemy target;
        if(targetIndex == -1) { target = null; } 
        else{ target = enemyList[targetIndex]; }

        // Below is regioned code for sending the appropriate action to the hero based on its class.
        // Sectioned off because it is VERY ugly.
        if(activeHero is HeroBarbarian heroBarbarian)
        {
            #region
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
        else if (activeHero is HeroRogue heroDruid)
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
        else if (activeHero is HeroRogue heroWizard)
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
            #endregion
        }
    }
    public void ProcessMinigameAndAction(HeroBarbarian hero, HeroAction action)
    {

    }
    
    public void AddPostTurnEvent(PostTurnEvent pte, GenericHero hero)
    {
        print("Hero Event! " + hero.name + " has " + pte);
        int index = GetHeroIndex(hero);
        Tuple<PostTurnEvent, int> t = new Tuple<PostTurnEvent, int>(pte, index);
        postTurnEvents.Add(t);
    }
    public void AddPostTurnEvent(PostTurnEvent pte, GenericEnemy enemy)
    {
        print("Enemy Event! " + enemy.name + " has " + pte);
        int index = GetEnemyIndex(enemy);
        Tuple<PostTurnEvent, int> t = new Tuple<PostTurnEvent, int>(pte, index);
        postTurnEvents.Add(t);
    }
    public void UpdateHeroHealthUI(GenericHero hero, int newHP)
    {
        if(hero == heroList[0])
        {
            bmManager.SetHeroOneHP(newHP);
        }
        else
        {
            bmManager.SetHeroTwoHP(newHP);
        }
    }
    public void UpdateHeroNotePointUI(GenericHero hero, int newNP)
    {
        if (hero == heroList[0])
        {
            bmManager.SetHeroOneNP(newNP);
        }
        else
        {
            bmManager.SetHeroTwoNP(newNP);
        }
    }
    public void UpdateEnemyHealthUI(GenericEnemy enemy, int newHP)
    {
        int index = GetEnemyIndex(enemy);
        bmManager.SetEnemyHPBarValue(index, newHP);
    }
    private bool OnHeroKilled(int index)
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
    private int GetEnemyIndex(GenericEnemy e)
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
    private int GetHeroIndex(GenericHero h)
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
    private void EndBattleWin()
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
        StopAllCoroutines();
        print("You Won The Battle!");
        playerController.SetControlType(ControlType.None);
        levelManager.LoadScene("TitleScreen");
    }
    private void EndBattleLose()
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
        StopAllCoroutines();
        print("You Lost The Battle!");
        playerController.SetControlType(ControlType.None);
        levelManager.LoadScene("TitleScreen");
    }
}
