using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuthenticateUI : MonoBehaviour {
    public static AuthenticateUI Instance { get; private set; }


    [SerializeField] private Button authenticateButton;
 
    public event EventHandler OnAuthenticated;


    private void Awake() {
        Instance = this;

        authenticateButton.onClick.AddListener(() => {
            EditPlayerName.Instance.UpdatePlayerName();
            CattibalLobbyManager.Instance.Authenticate(EditPlayerName.Instance.GetPlayerName());
            OnAuthenticated?.Invoke(this, EventArgs.Empty);
            Hide();
        });
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

}