using UnityEngine;
[CreateAssetMenu(fileName = "HeroAbility", menuName = "ScriptableObjects/HeroAbility", order = 2)]

public class HeroAbility : ScriptableObject
{
    public new string name;
    public string className;
    public string description;
    public int npCost;
    public bool requiresHeroSelection;
    public bool requiresEnemySelection;
    public GameObject minigame;
}
