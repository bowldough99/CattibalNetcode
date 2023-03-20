using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyAssets : MonoBehaviour {



    public static LobbyAssets Instance { get; private set; }


    [SerializeField] private Sprite tuxedoSprite;
    [SerializeField] private Sprite blueSprite;
    [SerializeField] private Sprite calicoSprite;
    [SerializeField] private Sprite siameseSprite;

    private void Awake() {
        Instance = this;
    }

    public Sprite GetSprite(CattibalLobbyManager.PlayerSkin playerSkin) {
        switch (playerSkin) {
            default:
            case CattibalLobbyManager.PlayerSkin.Tuxedo:   return tuxedoSprite;
            case CattibalLobbyManager.PlayerSkin.Blue:    return blueSprite;
            case CattibalLobbyManager.PlayerSkin.Calico:   return calicoSprite;
            case CattibalLobbyManager.PlayerSkin.Siamese:   return siameseSprite;
        }
    }

}