using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;
using TMPro;


public enum GameOverReason : byte
{
    None = 0,
    LastOneStanding = 1,
    Death = 2,
    Max,
}
public class CattibalGameManager : NetworkBehaviour
{
    public static CattibalGameManager Instance { get; private set; }
    public int numOfPlayers;
    public int totalPlayers = -1;
    public static GameObject[] spawnPoints;

    [Header("UI Settings")]
    public TextMeshProUGUI countdown;
    public TMP_Text gameTimerText;
    public TMP_Text gameOverText;
    public TMP_Text livesText;
    public TMP_Text itemSpawnedText;

    private bool ClientGameOver;
    private bool ClientGameStarted;
    private bool ClientStartCountdown;


    private float timeRemaining;
    public NetworkVariable<bool> isGameOver { get; } = new NetworkVariable<bool>(false);

    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private State state;
    private float waitingToStartTimer = 1f;
    private float countdownToStartTimer = 5f;
    private float gamePlayingTimer = 200f;
    private float itemSpawnerTimer = 5f;
    private bool canSpawnItem = true;

    public event EventHandler OnGameEnd;
    [SerializeField]private ItemManager itemManager;

    private void Awake()
    {
        Instance = this;

        state = State.WaitingToStart;
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawn");
        ShuffleSpawns();
    }

    internal static event Action OnInstanceReady;

    private void Update()
    {
        if (state == State.GameOver) return;
        Debug.Log(state);

        switch (state)
        {
            case State.WaitingToStart:
                if(numOfPlayers != totalPlayers)
                {
                    break;
                }
                waitingToStartTimer -= Time.deltaTime;
                if(waitingToStartTimer < 0f)
                {
                    state = State.CountdownToStart;
                }
                break;
            case State.CountdownToStart:
                countdown.gameObject.SetActive(true);
                countdownToStartTimer -= Time.deltaTime;
                countdown.text = ((int)countdownToStartTimer).ToString();
                if (countdownToStartTimer < 0f)
                {
                    countdown.gameObject.SetActive(false);
                    state = State.GamePlaying;
                }
                break;
            case State.GamePlaying:
                gameTimerText.gameObject.SetActive(true);
                gamePlayingTimer -= Time.deltaTime;
                UpdateGameTimer(gamePlayingTimer);
                itemSpawnerTimer -= Time.deltaTime;
                if (numOfPlayers <= 0)
                {
                    state = State.GameOver;
                    //but what if a player joined on his own?
                }
                if (NetworkManager.Singleton.IsServer && itemSpawnerTimer <= 0f && canSpawnItem == true)
                { 
                    itemManager.SpawnItems();
                    canSpawnItem = false;
                    NotifyItemSpawnClientRpc();
                }
                break;
            case State.GameOver:
                gameOverText.gameObject.SetActive(true);
                break;
        }
    }


    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }

    public void PauseGame()
    {
        // need to consider pausing for everyone
    }

    public void LeaveGame()
    {
        // if host, the whole game ends and returns everyone to main menu

        // if client, handle client disconnected
    }
    public void registerPlayer()
    {
        numOfPlayers++;
    }


    public void ShuffleSpawns()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            int rnd = UnityEngine.Random.Range(0, spawnPoints.Length);
            GameObject tempGO = spawnPoints[rnd];
            spawnPoints[rnd] = spawnPoints[i];
            spawnPoints[i] = tempGO;
        }
        Debug.Log(spawnPoints);
    }

    public Vector3 getSpawnPoint()
    {
        return spawnPoints[numOfPlayers].GetComponent<Transform>().position;
    }

    void UpdateGameTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        gameTimerText.text = String.Format("{0:00} : {1:00}", minutes, seconds);
    }

    [ClientRpc]
    public void NotifyItemSpawnClientRpc()
    {
        itemSpawnedText.gameObject.SetActive(true);
        StartCoroutine(DecayMessage(itemSpawnedText.gameObject, 3));
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
}
