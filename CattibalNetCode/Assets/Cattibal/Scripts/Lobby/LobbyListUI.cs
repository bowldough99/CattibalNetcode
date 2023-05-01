using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyListUI : MonoBehaviour {


    public static LobbyListUI Instance { get; private set; }



    [SerializeField] private Transform lobbySingleTemplate;
    [SerializeField] private Transform container;
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button returnToMainMenuButton;


    private string lobbyName;
    private bool isPrivate;
    private int maxPlayers;
    private CattibalLobbyManager.GameMode gameMode;

    private void Awake() {
        Instance = this;

        lobbySingleTemplate.gameObject.SetActive(false);

        refreshButton.onClick.AddListener(RefreshButtonClick);
        createLobbyButton.onClick.AddListener(CreateLobbyButtonClick);
        returnToMainMenuButton.onClick.AddListener(() =>
        {
            AuthenticationService.Instance.SignOut();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void Start() {
        CattibalLobbyManager.Instance.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
        CattibalLobbyManager.Instance.OnJoinedLobby += LobbyManager_OnJoinedLobby;
        CattibalLobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
        CattibalLobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnKickedFromLobby;
        CattibalLobbyManager.Instance.OnGameStarted += LobbyManager_OnGameStarted;
        CattibalLobbyManager.Instance.OnLobbyReset += LobbyManager_OnLeftLobby;
        AuthenticateUI.Instance.OnAuthenticated += LobbyManager_OnAuthenticated;

        Hide();
    }

    private void LobbyManager_OnGameStarted(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void LobbyManager_OnAuthenticated(object sender, System.EventArgs e)
    {
        Show();
    }

    private void LobbyManager_OnKickedFromLobby(object sender, CattibalLobbyManager.LobbyEventArgs e) {
        Show();
    }

    private void LobbyManager_OnLeftLobby(object sender, EventArgs e) {
        Show();
    }

    private void LobbyManager_OnJoinedLobby(object sender, CattibalLobbyManager.LobbyEventArgs e) {
        Hide();
    }

    private void LobbyManager_OnLobbyListChanged(object sender, CattibalLobbyManager.OnLobbyListChangedEventArgs e) {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList) {

        Queue<Transform> toDeleteList = new Queue<Transform>();

        foreach (Transform child in container) {
            if (child == lobbySingleTemplate) continue;
            toDeleteList.Enqueue(child);
        }

        while(toDeleteList.Count > 0)
        {
            Transform toDelete = toDeleteList.Dequeue();
            toDelete.SetParent(null);
            Destroy(toDeleteList.Dequeue().gameObject);
        }

        foreach (Lobby lobby in lobbyList) {
            Transform lobbySingleTransform = Instantiate(lobbySingleTemplate, container);
            lobbySingleTransform.gameObject.SetActive(true);
            LobbyListSingleUI lobbyListSingleUI = lobbySingleTransform.GetComponent<LobbyListSingleUI>();
            lobbyListSingleUI.UpdateLobby(lobby);
        }
    }

    private void RefreshButtonClick() {
        CattibalLobbyManager.Instance.RefreshLobbyList();
    }

    private void CreateLobbyButtonClick() {
        CattibalLobbyManager.Instance.CreateLobby(
            lobbyName = EditPlayerName.Instance.GetPlayerName() + "'s Lobby",
            maxPlayers = 4,
            isPrivate = false,
            gameMode = CattibalLobbyManager.GameMode.BattleRoyale
        );
        Hide();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }

}