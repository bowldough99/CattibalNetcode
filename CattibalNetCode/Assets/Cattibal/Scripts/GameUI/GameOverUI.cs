using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI instance;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public Image fade;
    public float lerpspeed = 2;
    public float fadeTarget = 0.5f;
    public TextMeshProUGUI gameoverText;
    public GameObject buttons;
    public float delay = 3.0f;
    bool activated = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        gameoverText.gameObject.SetActive(false);
        buttons.SetActive(false);
        Color fadecolor = fade.color;
        fadecolor.a = 0;
        fade.color = fadecolor;
        activated = false;
    }

    public void ActivateDefeat()
    {
        if(activated)
        {
            return;
        }
        activated = true;
        gameoverText.gameObject.SetActive(false);
        buttons.SetActive(false);
        gameoverText.text = "Defeat";
        StartCoroutine("StartFade");
    }

    public void ActivateVictory()
    {
        if (activated)
        {
            return;
        }
        activated = true;
        gameoverText.gameObject.SetActive(false);
        buttons.SetActive(false);
        gameoverText.text = "Victory!";
        StartCoroutine("StartFade");
    }

    IEnumerator StartFade()
    {
        Color fadecolor = fade.color;
        fadecolor.a = 0;
        fade.color = fadecolor;

        yield return new WaitForSeconds(delay);

        while(fadecolor.a < fadeTarget)
        {
            fadecolor.a += lerpspeed * Time.deltaTime;
            fade.color = fadecolor;
            yield return null;
        }

        fadecolor.a = fadeTarget;
        fade.color = fadecolor;
        gameoverText.gameObject.SetActive(true);
        buttons.SetActive(true);
    }

    public void LeaveGame()
    {
        CattibalLobbyManager.Instance.ResetLobby();
    }
}
