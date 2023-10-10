using UnityEngine;

[CreateAssetMenu(fileName = "BattleData", menuName = "ScriptableObjects/BattleData", order = 1)]
public class BattleData : ScriptableObject
{
    public string sceneName;
    public GameObject[] enemies;
    public float xpAwarded;
    public AudioClip music;
}
