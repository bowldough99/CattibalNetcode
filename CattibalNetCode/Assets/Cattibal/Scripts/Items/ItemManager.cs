using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private Transform[] itemSpawnPoints;
    [SerializeField] private GameObject[] itemsToSpawn;

    private void Start()
    {
        //SpawnItems();
    }
    public void SpawnItems()
    {
        for (int i = 0; i < itemSpawnPoints.Length; i++)
        {
            int randomItem = Random.Range(0, itemsToSpawn.Length);

            Instantiate(itemsToSpawn[randomItem], itemSpawnPoints[i].transform.position, Quaternion.identity);
        }
    }


}
