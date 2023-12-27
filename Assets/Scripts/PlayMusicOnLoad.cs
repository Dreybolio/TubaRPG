using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  This script tells the SoundManager to play the given song, 
 *  and then immediately decides it is not perfect, and therefore killing itself immediately.
 */
public class PlayMusicOnLoad : MonoBehaviour
{
    [SerializeField] private AudioClip intro;
    [SerializeField] private AudioClip music;

    [SerializeField] private float volume = 1;
    [SerializeField] private bool loop = true;

    private SoundManager soundManager;
    void Start()
    {
        soundManager = SoundManager.Instance;
        if(intro != null)
        {
            soundManager.PlayMusicWithIntro(intro, music, volume);
        }
        else
        {
            soundManager.PlayMusic(music, loop, volume);
        }
        Destroy(gameObject);
    }
}
