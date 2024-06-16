using System.Collections.Generic;
using UnityEngine;

namespace EmotionalBaggage {
public class TileSpawner : MonoBehaviour
{
    [SerializeField]
    private int tileStartCount = 10;
    [SerializeField]
    private int minimumStraightTiles = 3;
    [SerializeField]
    private int maximumStraightTiles = 15;
    [SerializeField]
    private GameObject startingTile;
    [SerializeField]
    private float minimumObstacleSpawnDistance = 10f;
    [SerializeField]
    private float obstaclefrequency = 0.5f;
    [SerializeField]
    private List<GameObject> turnTiles;
    [SerializeField]
    private List<GameObject> obstacles;

    private Vector3 currentTileLocation = Vector3.zero;
    private Vector3 currentTileDirection = Vector3.forward;
    private GameObject prevTile;
    private List<GameObject> currentTiles;
    private List<GameObject> currentObstacles;
    
    private float distanceSinceLastObstacle = 0f;
    

    private void Start(){
        currentTiles = new List<GameObject>();
        currentObstacles = new List<GameObject>();

        Random.InitState(System.DateTime.Now.Millisecond);

        for (int i = 0; i < tileStartCount; ++i) {
            SpawnTile(startingTile.GetComponent<Tile>());
        }

        SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>());
    }
    private void SpawnTile(Tile tile, bool spawnObstacle = false) {

        Quaternion newTileRotation = tile.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);

        prevTile = GameObject.Instantiate(tile.gameObject, currentTileLocation, newTileRotation);
        currentTiles.Add(prevTile);

        if (spawnObstacle && distanceSinceLastObstacle >= minimumObstacleSpawnDistance) {
            SpawnObstacle();
            distanceSinceLastObstacle = 0f;
        }

        if (tile.type == TileType.STRAIGHT) {
            currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection);
        }
        distanceSinceLastObstacle += prevTile.GetComponent<Renderer>().bounds.size.z;
    }

    private void DeletePreviousTiles() {   
        while (currentTiles.Count != 1) {
            GameObject tile = currentTiles[0];
            currentTiles.RemoveAt(0);
            Destroy(tile);
        }

        while (currentObstacles.Count != 0) {
            GameObject obstacle = currentObstacles[0];
            currentObstacles.RemoveAt(0);
            Destroy(obstacle);
        }
    }

    public void AddNewDirection(Vector3 direction) {
        currentTileDirection = direction;
        DeletePreviousTiles();

        Vector3 tilePlacementScale;
        if (prevTile.GetComponent<Tile>().type == TileType.SIDEWAYS) {
            tilePlacementScale = Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size / 2 + (Vector3.one * startingTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);
        }
        else {
            tilePlacementScale = Vector3.Scale((prevTile.GetComponent<Renderer>().bounds.size - (Vector3.one * 2)) + (Vector3.one * startingTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);
        }

        currentTileLocation += tilePlacementScale;

        int currentPathLength = Random.Range(minimumStraightTiles, maximumStraightTiles);

        for (int i = 0; i < currentPathLength; ++i) {
            SpawnTile(startingTile.GetComponent<Tile>(), (i == 0)? false : true);
        }

        SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>(), false);
    }

    private void SpawnObstacle(){
        if (Random.value > obstaclefrequency) return;

        GameObject obstaclePrefab = SelectRandomGameObjectFromList(obstacles);
        Quaternion newObjectRotation = obstaclePrefab.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);
        GameObject obstacle = Instantiate(obstaclePrefab, currentTileLocation, newObjectRotation);
        currentObstacles.Add(obstacle);
    }
    private GameObject SelectRandomGameObjectFromList(List<GameObject> list) {
        if (list.Count == 0) return null;

        return list[Random.Range(0, list.Count)];

    }
}

}