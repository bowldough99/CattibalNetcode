using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;

public class LobbyListSingleUI : MonoBehaviour {

    
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playersText;
    [SerializeField] private TextMeshProUGUI gameModeText;
    [SerializeField] private Button joinLobbyButton;


    private Lobby lobby;


    private void Awake() {
        GetComponent<Button>().onClick.AddListener(() => {
            CattibalLobbyManager.Instance.JoinLobby(lobby);
        });
        joinLobbyButton.onClick.AddListener(() => {
            CattibalLobbyManager.Instance.JoinLobby(lobby);
            //Hide();
        });

    }

    public void UpdateLobby(Lobby lobby) {
        this.lobby = lobby;

        lobbyNameText.text = lobby.Name;
        playersText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        gameModeText.text = lobby.Data[CattibalLobbyManager.KEY_GAME_MODE].Value;
    }


}