using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Only allow one instance of
    #region 
    private static GameManager instance;

    public static GameManager Instance
    {
        get { return instance; }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
    }
    #endregion
    private GameData gameData;

    // Should be Barbarian, Rogue, Druid, Wizard
    [SerializeField] private GameObject[] heroObjectRefs;
    [SerializeField] private Sprite[] heroIconRefs;
    private void Start()
    {
        gameData = GameData.Instance;
        LayerMask[] layersToIgnoreEachOther = new LayerMask[]
        {
            LayerMask.NameToLayer("HeroLeader"),
            LayerMask.NameToLayer("HeroFollower"),
            LayerMask.NameToLayer("Enemy")
        };
        UnityExtentions.IgnoreAllPhysicsBetweenLayers(layersToIgnoreEachOther);
    }
    public void Save()
    {
        SaveData data = new()
        {
            heroOneType = (int)gameData.heroOne_HeroType,
            heroOneLevel = gameData.heroOne_Level,
            heroOneHPAllocations = gameData.heroOne_HPAllocations,
            heroOneNPAllocations = gameData.heroOne_NPAllocations,

            heroTwoType = (int)gameData.heroTwo_HeroType,
            heroTwoLevel = gameData.heroTwo_Level,
            heroTwoHPAllocations = gameData.heroTwo_HPAllocations,
            heroTwoNPAllocations = gameData.heroTwo_NPAllocations
        };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.dataPath + "/Resources/savefile.json", json);
    }
    public void Load()
    {
        string json = File.ReadAllText(Application.dataPath + "/Resources/savefile.json");
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        gameData.heroOne_HeroType = (HeroType)data.heroOneType;
        gameData.heroOne_BattleObject = heroObjectRefs[data.heroOneType];
        gameData.heroOne_Icon = heroIconRefs[data.heroOneType];
        gameData.heroOne_Level = data.heroOneLevel;
        gameData.heroOne_HPAllocations = data.heroOneHPAllocations;
        gameData.heroOne_NPAllocations = data.heroOneNPAllocations;

        gameData.heroTwo_HeroType = (HeroType)data.heroTwoType;
        gameData.heroTwo_BattleObject = heroObjectRefs[data.heroTwoType];
        gameData.heroTwo_Icon = heroIconRefs[data.heroTwoType];
        gameData.heroTwo_Level = data.heroTwoLevel;
        gameData.heroTwo_HPAllocations = data.heroTwoHPAllocations;
        gameData.heroTwo_NPAllocations = data.heroTwoNPAllocations;
    }
}
