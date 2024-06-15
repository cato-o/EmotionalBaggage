using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltTile : MonoBehaviour
{
    private BeltSpawner beltSpawner;
    [SerializeField]
    private GameObject obstaclePrefab;


    void Start()
    {
        GameObject temp = GameObject.FindGameObjectWithTag("Spawner");
        beltSpawner = temp.GetComponent<BeltSpawner>();
        if (beltSpawner == null)
        {
            Debug.Log("belt spawner null");
        }
        spawnObstacle();
    }

    // spawn new tile at end when leave tile and destroys old tile
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exited belt tile");
        beltSpawner.SpawnTile();
        Destroy(gameObject, 2);
    }

    // spawns an obstacle randomly in the left, center, or middle
    void spawnObstacle()
    {
        int obstacleSpawnIndex = Random.Range(2, 5);
        Transform spawnPoint = transform.GetChild(obstacleSpawnIndex).transform;

        Instantiate(obstaclePrefab, spawnPoint.position, Quaternion.identity, transform);
    }
}
