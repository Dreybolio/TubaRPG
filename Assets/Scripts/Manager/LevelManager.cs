using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;

    public static LevelManager Instance
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

    public void LoadBattle(BattleData bd)
    {
        StartCoroutine(C_LoadBattle(bd));
    }
    private IEnumerator C_LoadBattle(BattleData bd)
    {
        var asyncSceneLoad = SceneManager.LoadSceneAsync(bd.sceneName, LoadSceneMode.Single);
        while(!asyncSceneLoad.isDone)
        {
            yield return null;
        }
        // Once scene is fully loaded
        FindObjectOfType<BattleManager>().SetBattleData(bd);
    }
    public void LoadScene(string sceneId)
    {
        StartCoroutine (C_LoadScene(sceneId));
    }
    private IEnumerator C_LoadScene(string sceneId)
    {
        var asyncSceneLoad = SceneManager.LoadSceneAsync(sceneId, LoadSceneMode.Single);
        while (!asyncSceneLoad.isDone)
        {
            yield return null;
        }
    }
}
