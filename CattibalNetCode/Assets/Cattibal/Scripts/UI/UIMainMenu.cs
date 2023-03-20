using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{

    [SerializeField] Button playButton;
    [SerializeField] Button tutorialButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button quitButton;

    private void Awake()
    {
        //this is a form of delegate lambda
        playButton.onClick.AddListener(() =>
        {
            playButton.interactable = false;
            Loader.Load(Loader.Scene.GameScene);
        });
        tutorialButton.onClick.AddListener(() =>
        {

        });
        settingsButton.onClick.AddListener(() =>
        {

        });
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

}
