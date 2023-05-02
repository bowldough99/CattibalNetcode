using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource songPlayer;
    public AudioClip mainMenuSong;
    public AudioClip inGameSong;
    public AudioClip buttonClickSFX;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        songPlayer = GetComponent<AudioSource>();
        songPlayer.clip = mainMenuSong;
        songPlayer.Play();
    }
    public void PlayInGameSong()
    {
        songPlayer.clip = inGameSong;
        songPlayer.Play();
    }
    public void PlayMainMenuSong()
    {
        songPlayer.clip = mainMenuSong;
        songPlayer.Play();
    }

}
