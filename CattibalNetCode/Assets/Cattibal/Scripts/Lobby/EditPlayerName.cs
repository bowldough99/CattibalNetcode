using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class EditPlayerName : MonoBehaviour {


    public static EditPlayerName Instance { get; private set; }


    public event EventHandler OnNameChanged;


    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TMP_InputField inputField;


    private string playerName = "noob";


    private void Awake() {
        Instance = this;

        inputField = transform.Find("inputField").GetComponent<TMP_InputField>();

        playerNameText.text = playerName;
    }

    private void Start() {
        OnNameChanged += EditPlayerName_OnNameChanged;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            UpdatePlayerName();
        }
    }
    private void EditPlayerName_OnNameChanged(object sender, EventArgs e) {
        CattibalLobbyManager.Instance.UpdatePlayerName(GetPlayerName());
    }

    public string GetPlayerName() {
        return playerName;
    }

    public void UpdatePlayerName()
    {
        playerName = inputField.text;
    }

    private char ValidateChar(string validCharacters, char addedChar)
    {
        if (validCharacters.IndexOf(addedChar) != -1)
        {
            // Valid
            return addedChar;
        }
        else
        {
            // Invalid
            return '\0';
        }
    }

    private void Show(string inputString, string validCharacters, int characterLimit)
    {
        inputField.characterLimit = characterLimit;
        inputField.onValidateInput = (string text, int charIndex, char addedChar) => {
            return ValidateChar(validCharacters, addedChar);
        };

        inputField.text = inputString;
        inputField.Select();
    }

    public static void Show_Static(string inputString, string validCharacters, int characterLimit)
    {
        Instance.Show(inputString, validCharacters, characterLimit);
    }

    public static void Show_Static(int defaultInt)
    {
        Instance.Show(defaultInt.ToString(), "0123456789-", 20);
    }


}