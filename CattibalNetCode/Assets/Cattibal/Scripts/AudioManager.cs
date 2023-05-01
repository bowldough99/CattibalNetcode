using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioManager Instance { get; private set; }
    public AudioClip mainMenuSong;
    public AudioClip inGameSong;
    public AudioClip buttonClickSFX;

    private void Awake()
    {
        Instance = this;
    }
}
