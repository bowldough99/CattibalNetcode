using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ItemManager : NetworkBehaviour
{
    public static ItemManager Instance { get; private set; }

    [SerializeField] private Transform[] itemSpawnPoints;
    [SerializeField] private GameObject[] itemsToSpawn;

    private void Awake()
    {
        Instance = this; 
    }
    public void SpawnItems()
    {
        for (int i = 0; i < itemSpawnPoints.Length; i++)
        {
            int randomItem = Random.Range(0, itemsToSpawn.Length);
            GameObject weapons = Instantiate(itemsToSpawn[randomItem], itemSpawnPoints[i].transform.position, Quaternion.identity);
            weapons.GetComponent<NetworkObject>().Spawn();
        }
    }


}
