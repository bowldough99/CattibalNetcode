using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            GameObject.DontDestroyOnLoad(this);
        }

    }


    public enum Scene
    {
        MainMenu,
        Playground
    }
    public void LoadScene(Scene scene)
    {
        //Debug.Log("sceneName to load: " + scenename);
        SceneManager.LoadScene(scene.ToString());
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(Scene.MainMenu.ToString());

    }

}