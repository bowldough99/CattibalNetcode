using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {

    public static LobbyUI Instance { get; private set; }

    //public event EventHandler OnGameStarted;

    [SerializeField] private Transform playerSingleTemplate;
    [SerializeField] private Transform container;
    //[SerializeField] private TextMeshProUGUI lobbyNameText;
    //[SerializeField] private TextMeshProUGUI playerCountText;
    //[SerializeField] private TextMeshProUGUI gameModeText;
    [SerializeField] private Button changeBlueButton;
    [SerializeField] private Button changeCalicoButton;
    [SerializeField] private Button changeTuxedoButton;
    [SerializeField] private Button changeSiameseButton;
    [SerializeField] private Button leaveLobbyButton;
    //[SerializeField] private Button changeGameModeButton;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Image lobbyBackground;

    private bool GameStarted = false;


    private void Awake() {
        Instance = this;

        playerSingleTemplate.gameObject.SetActive(false);

        changeBlueButton.onClick.AddListener(() => {
            CattibalLobbyManager.Instance.UpdatePlayerCharacter(CattibalLobbyManager.PlayerSkin.Blue);
        });
        changeCalicoButton.onClick.AddListener(() => {
            CattibalLobbyManager.Instance.UpdatePlayerCharacter(CattibalLobbyManager.PlayerSkin.Calico);
        });
        changeTuxedoButton.onClick.AddListener(() => {
            CattibalLobbyManager.Instance.UpdatePlayerCharacter(CattibalLobbyManager.PlayerSkin.Tuxedo);
        });
        changeSiameseButton.onClick.AddListener(() => {
            CattibalLobbyManager.Instance.UpdatePlayerCharacter(CattibalLobbyManager.PlayerSkin.Siamese);
        });

        leaveLobbyButton.onClick.AddListener(() => {
            CattibalLobbyManager.Instance.LeaveLobby();
        });

        //changeGameModeButton.onClick.AddListener(() => {
        //    CattibalLobbyManager.Instance.ChangeGameMode();
        //});

        startGameButton.onClick.AddListener(() =>
        {
            Debug.Log("Start clicked");
            startGameButton.gameObject.SetActive(false);
            CattibalLobbyManager.Instance.StartGame();
            GameStarted = true;
        });
    }

    private void Start() {
        CattibalLobbyManager.Instance.OnJoinedLobby += UpdateLobby_Event;
        CattibalLobbyManager.Instance.OnJoinedLobbyUpdate += UpdateLobby_Event;
        CattibalLobbyManager.Instance.OnLobbyGameModeChanged += UpdateLobby_Event;
        CattibalLobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
        CattibalLobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnLeftLobby;
        CattibalLobbyManager.Instance.OnGameStarted += LobbyManager_OnGameStarted;
        CattibalLobbyManager.Instance.OnLobbyReset += LobbyManager_ResetLobby;
        Hide();
    }

    private void LobbyManager_OnGameStarted(object sender, System.EventArgs e)
    {
        ClearLobby();
        lobbyBackground.gameObject.SetActive(false);
        Hide();
    }

    private void LobbyManager_OnLeftLobby(object sender, System.EventArgs e) {
        ClearLobby();
        Hide();
    }
    private void LobbyManager_ResetLobby(object send, EventArgs e)
    {
        ClearLobby();
        Hide();
        lobbyBackground.gameObject.SetActive(true);
    }
    private void SetStartGameButtonVisible(bool visible)
    {
        if(startGameButton != null)
        {
            startGameButton.gameObject.SetActive(visible);
        }
    }

    private void UpdateLobby_Event(object sender, CattibalLobbyManager.LobbyEventArgs e) {
        UpdateLobby();
    }

    private void UpdateLobby() {
        UpdateLobby(CattibalLobbyManager.Instance.GetJoinedLobby());
    }

    private void UpdateLobby(Lobby lobby) {
        ClearLobby();

        foreach (Player player in lobby.Players) {
            Transform playerSingleTransform = Instantiate(playerSingleTemplate, container);
            playerSingleTransform.gameObject.SetActive(true);
            LobbyPlayerSingleUI lobbyPlayerSingleUI = playerSingleTransform.GetComponent<LobbyPlayerSingleUI>();

            lobbyPlayerSingleUI.SetKickPlayerButtonVisible(
                CattibalLobbyManager.Instance.IsLobbyHost() &&
                player.Id != AuthenticationService.Instance.PlayerId // Don't allow kick self
            );

            SetStartGameButtonVisible(
            CattibalLobbyManager.Instance.IsLobbyHost() && !GameStarted
            );

            lobbyPlayerSingleUI.UpdatePlayer(player);
        }

        //changeGameModeButton.gameObject.SetActive(CattibalLobbyManager.Instance.IsLobbyHost());

        //lobbyNameText.text = lobby.Name;
        //playerCountText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        //gameModeText.text = lobby.Data[CattibalLobbyManager.KEY_GAME_MODE].Value;

        Show();
    }

    private void ClearLobby() {
        foreach (Transform child in container) {
            if (child == playerSingleTemplate) continue;
            Destroy(child.gameObject);
        }
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }
}