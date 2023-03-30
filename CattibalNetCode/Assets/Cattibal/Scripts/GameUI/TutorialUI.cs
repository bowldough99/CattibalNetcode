using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : NetworkBehaviour
{
    public int lerpspeed = 2;
    public float delayTimer = 2;
    float scaleTimer = 0;

    public Image fadeImage;
    public float fadeTarget = 0.5f;

    public GameObject tutorial;
    TutorialState state = TutorialState.OPEN;
    public PlayerNetwork player;

    enum TutorialState
    {
        OPEN,
        CLOSE
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;
            return;
        }

        if (state == TutorialState.OPEN)
            OpenTutorial();
        else
            CloseTutorial();
    }

    void OpenTutorial()
    {
        Color fadecolor = fadeImage.color;
        if(fadecolor.a >= fadeTarget)
        {
            tutorial.SetActive(true);
            return;
        }
        scaleTimer += Time.deltaTime * lerpspeed * Mathf.PI;
        scaleTimer = Mathf.Clamp(scaleTimer, 0, Mathf.PI / 2);

        fadecolor.a = Mathf.Clamp(Mathf.Sin(scaleTimer), 0, 1);
        fadeImage.color = fadecolor;
    }

    void CloseTutorial()
    {
        Color fadecolor = fadeImage.color;
        if (fadecolor.a <= Mathf.Epsilon)
        {
            gameObject.SetActive(false);
            return;
        }
        scaleTimer -= Time.deltaTime * lerpspeed * Mathf.PI;
        scaleTimer = Mathf.Clamp(scaleTimer, 0, Mathf.PI / 2);

        fadecolor.a = Mathf.Clamp(Mathf.Sin(scaleTimer), 0, 1);
        fadeImage.color = fadecolor;
    }

    public void ReadyButton()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            CattibalGameManager.Instance.registerPlayer();
            if (CattibalGameManager.Instance.numOfPlayers == CattibalGameManager.Instance.totalPlayers)
                player.NotifyClientsReadyClientRpc();
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            Debug.Log("notify server i is ready");
            player.NotifyReadyServerRpc();
        }

        state = TutorialState.CLOSE;
    }

}
