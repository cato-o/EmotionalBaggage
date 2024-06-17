using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltSpawner : MonoBehaviour
{
    public GameObject beltTile;
    Vector3 spawnPoint;

    private int count;

    // spawns new tile at spawnpoint
    public void SpawnTile()
    {
        GameObject currentTile = Instantiate(beltTile, spawnPoint, Quaternion.identity);
        spawnPoint = currentTile.transform.GetChild(1).transform.position;
        Debug.Log("tile spawned" + count);
        count++;
    }

    void Start()
    {
        count = 0;
        // spawns 10 tiles initially
        for(int i = 0; i < 10; i++)
        {
            SpawnTile();
        }
    }


}
