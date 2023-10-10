using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClassType
{
    Barbarian,
    Rogue,
    Druid,
    Wizard
}
public class GameData : MonoBehaviour
{
    // Only allow one instance of
    #region 
    private static GameData instance;

    public static GameData Instance
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
        }
    }
    #endregion

    public int heroOne_Level;
    public int heroOne_HPAllocations;
    public int heroOne_NPAllocations;
    public ClassType heroOne_ClassType;
    public GameObject heroOne_Object;
    public Sprite heroOne_Icon;

    public int heroTwo_Level;
    public int heroTwo_HPAllocations;
    public int heroTwo_NPAllocations;
    public ClassType heroTwo_ClassType;
    public GameObject heroTwo_Object;
    public Sprite heroTwo_Icon;

    public Item[] items;
}
