using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldManager : MonoBehaviour
{
    private GameData gameData;

    [SerializeField] private OverworldHeroController heroLeader;
    [SerializeField] private OverworldHeroFollower heroFollower;

    private void Start()
    {
        gameData = GameData.Instance;

        SetCharacterModels();
    }

    private void SetCharacterModels()
    {
        heroLeader.SetCharacterModel(gameData.heroOne_CharacterModel, gameData.heroOne_HeroType);
        heroFollower.SetCharacterModel(gameData.heroTwo_CharacterModel, gameData.heroTwo_HeroType);
    }
}
