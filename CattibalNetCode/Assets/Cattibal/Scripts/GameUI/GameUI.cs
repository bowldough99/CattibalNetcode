using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }

    //public event EventHandler OnGameStarted;

    [SerializeField] private Transform healthBar;
    [SerializeField] private Transform hungerBar;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CattibalLobbyManager.Instance.OnGameStarted += GameManager_OnGameStarted;
        CattibalGameManager.Instance.OnGameEnd += GameManager_OnGameEnded;

        Hide();
    }

    private void GameManager_OnGameStarted(object sender, System.EventArgs e)
    {
        Show();
    }    
    
    private void GameManager_OnGameEnded(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

}
