using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }

    //public event EventHandler OnGameStarted;

    [SerializeField] private Transform healthBar;
    [SerializeField] private Transform hungerBar;

    public GameObject pickUpMessagePrefab;
    private GameObject tempObj;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CattibalLobbyManager.Instance.OnGameStarted += GameManager_OnGameStarted;
        CattibalGameManager.Instance.OnGameEnd += GameManager_OnGameEnded;

        Hide();
    }


    public void AddCanPickUpMessage(string message)
    {
        QueueMessage(message);
    }
    public void QueueMessage(string message)
    {
        if(tempObj == null)
        {
            tempObj = Instantiate(pickUpMessagePrefab);
            tempObj.GetComponent<TextMeshProUGUI>().text = message;

            tempObj.transform.SetParent(transform);

            tempObj.transform.localPosition = Vector3.zero;
            StartCoroutine(DecayMessage(tempObj, 0.5f));

        }
    }

    IEnumerator DecayMessage(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();
        while (text.color.a > 0)
        {
            text.color -= new Color(0, 0, 0, 1) * Time.deltaTime;
            yield return null;
        }
        Destroy(obj);
    }
    private void GameManager_OnGameStarted(object sender, System.EventArgs e)
    {
        Show();
    }    
    
    private void GameManager_OnGameEnded(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

}
