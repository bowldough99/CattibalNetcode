using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
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
    public Image itemSpawnedTextBox;
    public GameObject gameTimer;
    public TMP_Text waitingForPlayersText;

    private bool ClientGameOver;
    private bool ClientGameStarted;
    private bool ClientStartCountdown;


    private float timeRemaining;
    public NetworkVariable<bool> isGameOver { get; } = new NetworkVariable<bool>(false);

    HashSet<ulong> deadplayers = new HashSet<ulong>();

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
    [SerializeField] private ItemManager itemManager;

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
                waitingForPlayersText.color = new Color(1f, 1f, 1f, Mathf.PingPong(Time.time * 0.5f, 1));
                deadplayers.Clear();
                countdown.text = "";
                gameTimerText.text = "";
                if (numOfPlayers != totalPlayers)
                {
                    break;
                }
                waitingToStartTimer -= Time.deltaTime;
                if (waitingToStartTimer < 0f)
                {
                    state = State.CountdownToStart;
                }
                break;
            case State.CountdownToStart:
                waitingForPlayersText.gameObject.SetActive(false);
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
                gameTimer.gameObject.SetActive(true);
                gamePlayingTimer -= Time.deltaTime;
                UpdateGameTimer(gamePlayingTimer);
                itemSpawnerTimer -= Time.deltaTime;
                if (numOfPlayers <= 1)
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

    public void registerPlayer()
    {
        Debug.Log("player redy");
        numOfPlayers++;
    }

    public bool HasGameTimerEned()
    {
        return gamePlayingTimer <= 0;
    }


    public void ShuffleSpawns()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            int rnd = Random.Range(0, spawnPoints.Length);
            GameObject tempGO = spawnPoints[rnd];
            spawnPoints[rnd] = spawnPoints[i];
            spawnPoints[i] = tempGO;
        }
        Debug.Log(spawnPoints);
    }

    public Vector3 getSpawnPoint(int id)
    {
        return spawnPoints[id % spawnPoints.Length].GetComponent<Transform>().position;
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
        itemSpawnedTextBox.gameObject.SetActive(true);
        StartCoroutine(DecayImage(itemSpawnedTextBox.gameObject, 3));
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
    IEnumerator DecayImage(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Image image = obj.GetComponent<Image>();
        while (image.color.a > 0)
        {
            image.color -= new Color(0, 0, 0, 1) * Time.deltaTime;
            yield return null;
        }
        Destroy(obj);
    }

    public void KillPlayer(ulong clientid)
    {
        if(deadplayers.Contains(clientid))
        {
            return;
        }
        Debug.Log("player ded");
        numOfPlayers--;
        if (numOfPlayers < 0)
            numOfPlayers = 0;
        deadplayers.Add(clientid);
    }

    public bool IsWinner(ulong clientid)
    {
        Debug.Log(string.Format("is ded {0}", deadplayers.Contains(clientid)));
        return state == State.GameOver && !deadplayers.Contains(clientid);
    }

    public void ResetGameManager()
    {
        numOfPlayers = 0;
        totalPlayers = -1;
        state = State.WaitingToStart;
        deadplayers = new HashSet<ulong>();
        isGameOver.Value = false;


        waitingToStartTimer = 1f;
        countdownToStartTimer = 5f;
        gamePlayingTimer = 200f;
        itemSpawnerTimer = 5f;
    }
}
