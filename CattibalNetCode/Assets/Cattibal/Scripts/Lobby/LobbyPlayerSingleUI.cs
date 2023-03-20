using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;

public class LobbyPlayerSingleUI : MonoBehaviour {


    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Image characterImage;
    [SerializeField] private Button kickPlayerButton;
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private Transform characterLocation;

    private SkinnedMeshRenderer characterRenderer;
    private GameObject character;
    public Material skinToChangeTo;

    [SerializeField] private Material[] sharedMaterials;
    private Player player;


    private void Awake() {
        kickPlayerButton.onClick.AddListener(KickPlayer);
    }

    public void SetKickPlayerButtonVisible(bool visible) {
        kickPlayerButton.gameObject.SetActive(visible);
    }

    public void UpdatePlayer(Player player) {
        this.player = player;
        playerNameText.text = player.Data[CattibalLobbyManager.KEY_PLAYER_NAME].Value;
        CattibalLobbyManager.PlayerSkin playerSkin = 
            System.Enum.Parse<CattibalLobbyManager.PlayerSkin>(player.Data[CattibalLobbyManager.KEY_PLAYER_SKIN].Value);
        characterImage.sprite = LobbyAssets.Instance.GetSprite(playerSkin);
        SetUpCatSkin();
        skinToChangeTo = GetMaterial(playerSkin);
        characterRenderer.material = skinToChangeTo;
    }

    private void KickPlayer() {
        if (player != null) {
            CattibalLobbyManager.Instance.KickPlayer(player.Id);
        }
    }

    private void SetUpCatSkin()
    {
        character = Instantiate(characterPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        character.transform.SetParent(characterLocation.transform, false);
        characterRenderer = character.GetComponent<SkinnedMeshRenderer>();
    }

    private Material GetMaterial(CattibalLobbyManager.PlayerSkin playerSkin)
    {
        switch (playerSkin)
        {
            default:
            case CattibalLobbyManager.PlayerSkin.Tuxedo: return sharedMaterials[0];
            case CattibalLobbyManager.PlayerSkin.Blue: return sharedMaterials[1];
            case CattibalLobbyManager.PlayerSkin.Calico: return sharedMaterials[2];
            case CattibalLobbyManager.PlayerSkin.Siamese: return sharedMaterials[3];
        }
    }


}