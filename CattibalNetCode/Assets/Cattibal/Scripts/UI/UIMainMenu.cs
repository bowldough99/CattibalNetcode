using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{

    [SerializeField] Button playButton;
    [SerializeField] Button tutorialButton;
    //[SerializeField] Button settingsButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button backButton;

    [SerializeField] GameObject mainMenuUI;
    [SerializeField] GameObject tutorialUI;
    //public SoundManager soundManager;

    private void Awake()
    {
        //if (soundManager == null)
        //{
        //    soundManager = GameObject.FindObjectOfType<SoundManager>(true);
        //}
        //this is a form of delegate lambda
        playButton.onClick.AddListener(() =>
        {
            playButton.interactable = false;
            //soundManager.PlayInGameSong();
            Loader.Load(Loader.Scene.GameScene);
        });
        tutorialButton.onClick.AddListener(() =>
        {
            tutorialUI.SetActive(true);
            mainMenuUI.SetActive(false);
        });
        //settingsButton.onClick.AddListener(() =>
        //{

        //});
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        backButton.onClick.AddListener(() =>
        {
            tutorialUI.SetActive(false);
            mainMenuUI.SetActive(true);
        });
    }

}
