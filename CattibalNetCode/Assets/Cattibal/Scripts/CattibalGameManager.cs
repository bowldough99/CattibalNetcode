using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CattibalGameManager : MonoBehaviour
{
    public static CattibalGameManager Instance { get; private set; }
    public int numOfPlayers;
    public static GameObject[] spawnPoints;

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
    private float gamePlayingTimer = 10f;
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

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                waitingToStartTimer -= Time.deltaTime;
                if(waitingToStartTimer < 0f)
                {
                    state = State.CountdownToStart;
                }
                break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer < 0f)
                {
                    state = State.GamePlaying;
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                itemSpawnerTimer -= Time.deltaTime;
                if (numOfPlayers <= 0)
                {
                    state = State.GameOver;
                    //but what if a player joined on his own?
                }
                if (itemSpawnerTimer <= 0f && canSpawnItem == true)
                {
                    itemManager.SpawnItems();
                    canSpawnItem = false;
                    Debug.Log("Spawning Items");
                }
                break;
            case State.GameOver:
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
}
